using System;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;

namespace Assets.Scripts
{
    //TODO: should we disable exit listener on setup?
    /// <summary>
    /// Play mode using network.
    /// Host is chosen based on the Participant ID (said to be practically random).
    /// Then host generates the board and sends it to the opponent.
    /// Initialization never calls base methods, since it is completely different.
    /// </summary>
    public class NetworkPlayGameState : PlayGameState
    {
        [SerializeField] private NetworkInputListener networkInputListener;
        private MultiplayerController multiplayerController;
        private List<Participant> allPlayers;
        private bool isHost;
        private TurnType myTurnType;
        private UserResultType currentUserResultType;
        private string myId;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void InvokeState()
        {
            inputListener = networkInputListener;
            SetupGame();
            
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnPlayerLeft += ProcessPlayerLeft;

            string hostId;
            //Maybe we do not need this try/catch anymore
            try
            {
                allPlayers = multiplayerController.GetAllPlayers();
                hostId = ChooseHost();
                myId = multiplayerController.GetMyParticipantId();
            }
            catch (Exception)
            {
                multiplayerController.LeaveRoom();
                stateManager.ChangeState(StateType.LOBBY_GAME_STATE);
                return;
            }

            isHost = false;
            
            //Let the host set up the round.
            if (hostId == myId)
            {
                isHost = true;
                SetupRoundHost();
                myTurnType = TurnType.FIRST;
                InitNewRound();
            }
            else
            {
                myTurnType = TurnType.SECOND;
                multiplayerController.OnMessageReceived += SetupRoundFromNetwork;
            }
        }
        
        private string ChooseHost()
        {
            return allPlayers.Min(participant => participant.ParticipantId);
        }

        private void SetupRoundHost()
        {
            boardFactory.FillBoardStorageRandomly(boardStorage, ConfigurationType, boardManager);

            var message = boardFactory.ConvertBoardStorageToBytes(boardStorage);
            
            //Insert 'S' -- Setup message.
            message.Insert(0, (byte) 'S');

            multiplayerController.SendMessage(message.ToArray());
           
            FinishSetup();
        }
        
        private void SetupRoundFromNetwork(byte[] message)
        {
            if (message[0] != 'S')
            {
                return;
            }
 
            multiplayerController.OnMessageReceived -= SetupRoundFromNetwork;

            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray(), boardStorage, ConfigurationType, boardManager);
            FinishSetup();
            
            InitNewRound();
            
            //Because host is the first to move
            boardStorage.InvertBoard();
        }

        /// <summary>
        /// Common initialization after round setup.
        /// </summary>
        private void FinishSetup()
        {
            board.SetInputListener(networkInputListener);
            var firstController = new UserController(PlayerType.FIRST, boardStorage, boardFactory, this, 
                armyText, roundEffects);
            var secondController = new UserController(PlayerType.SECOND, boardStorage, boardFactory, this, 
                armyText, roundEffects);

            if (isHost)
            {
                secondController.ArmyFinishedMove += OnOpponentFinishedArmyFinishedMove;
            }
            else
            {
                firstController.ArmyFinishedMove += OnOpponentFinishedArmyFinishedMove;
            }
            
            controllerManager = new ControllerManager(firstController, secondController);
            networkInputListener.Initialize(controllerManager, board.Width, board.Height);
            playMenu.Initialize(boardManager, inputListener);
            turnManager.Initialize(boardManager, controllerManager);
        }

        private void OnOpponentFinishedArmyFinishedMove()
        {
            boardStorage.DisableBoardButtons();
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            if (turnManager.CurrentTurn != myTurnType)
            {
                playMenu.DisableUI();
            }
            else
            {
                playMenu.EnableUI();
            }
        }

        protected override TurnType GetFirstTurn()
        {
            //Host makes first turn (as PlayerType.First).
            return TurnType.FIRST;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && stateManager.CurrentState is NetworkPlayGameState)
            {
                ProcessPlayerLeft("");
            }
        }

        private void ProcessPlayerLeft(string message)
        {
            if (myId == message || message == "")
            {
                currentUserResultType = UserResultType.LOSE;
            }
            else
            {
                currentUserResultType = UserResultType.WIN;
                multiplayerController.LeaveRoom();
            }
            CloseGame();
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Displays an appropriate text to the user.
        /// </summary>
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            
            if (resultType == ResultType.FIRST_WIN)
            {
                if (isHost)
                {
                    //We are the host (the first), so we win.
                    lerpedText.PerformLerpString("You win!", Color.green);
                    currentUserResultType = UserResultType.WIN;
                }
                else
                {
                    //We are not the host (the second), so we lose.
                    lerpedText.PerformLerpString("You lose...", Color.red);
                    currentUserResultType = UserResultType.LOSE;
                }
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                if (!isHost)
                {
                    //We are not the host (the first), so we win.
                    lerpedText.PerformLerpString("You win!", Color.green);
                    currentUserResultType = UserResultType.WIN;
                    
                }
                else
                {
                    //We are host (the first), so we lose.
                    lerpedText.PerformLerpString("You lose...", Color.red);
                    currentUserResultType = UserResultType.LOSE;
                }
            }
            else if (resultType == ResultType.DRAW)
            {
                lerpedText.PerformLerpString("Draw", Color.blue);
                currentUserResultType = UserResultType.DRAW;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void CloseGame()
        {
            if (isHost)
            {
                controllerManager.SecondController.ArmyFinishedMove -= OnOpponentFinishedArmyFinishedMove;
            }
            else
            {
                controllerManager.FirstController.ArmyFinishedMove -= OnOpponentFinishedArmyFinishedMove;
            }
            
            multiplayerController.OnPlayerLeft -= ProcessPlayerLeft;
            multiplayerController.LeaveRoom();
            stateManager.ResultGameState.Initialize(currentUserResultType, playMode);
            stateManager.ChangeState(StateType.RESULT_GAME_STATE);
        }

        protected override void OnBackButtonPressed()
        {
            multiplayerController.LeaveRoom();
            //Further ProcessPlayerLeft will be invoked.
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void CloseState()
        {
            networkInputListener.Stop();
            base.CloseState();
        }

        /// <inheritdoc />
        /// <summary>
        /// Host player is the first, another one is the second.
        /// </summary>
        protected override void InitNewRound()
        {
            base.InitNewRound();
            
            if ((turnManager.CurrentTurn == TurnType.FIRST && isHost) || 
                (turnManager.CurrentTurn == TurnType.SECOND && !isHost))
            {
                playMenu.EnableUI();
            }
            else
            {
                playMenu.DisableUI();
            }
        }
    }
}