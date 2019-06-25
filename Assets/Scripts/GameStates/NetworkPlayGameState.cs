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

        private const byte setupMessageByte = (byte) 'S';
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void InvokeState()
        {
            //Change the default input listener.
            inputListener = networkInputListener;
            //Perform common initialization.
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
        
        /// <summary>
        /// The host is chosen based on the Participant ID (said to be practically random).
        /// </summary>
        private string ChooseHost()
        {
            return allPlayers.Min(participant => participant.ParticipantId);
        }

        private void SetupRoundHost()
        {
            //Create random board and serialize it.
            boardFactory.FillBoardStorageRandomly(boardStorage, ConfigurationType, boardManager);
            var message = boardFactory.ConvertBoardStorageToBytes(boardStorage);
            
            //Insert 'S' -- Setup leftParticipantId.
            message.Insert(0, setupMessageByte);
            //Send the board to the opponent.
            multiplayerController.SendMessage(message.ToArray());
           
            FinishSetup();
        }
        
        private void SetupRoundFromNetwork(byte[] message)
        {
            if (message[0] != setupMessageByte)
            {
                return;
            }
 
            multiplayerController.OnMessageReceived -= SetupRoundFromNetwork;

            //Skip the first byte, since it is an indicator of the leftParticipantId type (setup).
            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray(), boardStorage, ConfigurationType, 
                boardManager);
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
                secondController.ArmyFinishedMove += OnOpponentArmyFinishedMove;
            }
            else
            {
                firstController.ArmyFinishedMove += OnOpponentArmyFinishedMove;
            }
            
            controllerManager = new ControllerManager(firstController, secondController);
            networkInputListener.Initialize(controllerManager, board.Width, board.Height);
            playMenu.Initialize(boardManager, inputListener);
            turnManager.Initialize(boardManager, controllerManager);
        }

        private void OnOpponentArmyFinishedMove()
        {
            boardStorage.DisableBoardButtons();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override TurnType GetFirstTurn()
        {
            //Host makes first turn (as PlayerType.First).
            return TurnType.FIRST;
        }

        /// <summary>
        /// Processes the pause mode or hiding in a tray on mobile devices.
        /// The user who did that loses. Another one wins.
        /// </summary>
        private void OnApplicationPause(bool pause)
        {
            if (pause && stateManager.CurrentState is NetworkPlayGameState)
            {
                //We did it, and at this moment we do not know our id anymore.
                ProcessPlayerLeft("");
            }
        }

        /// <summary>
        /// Determines the winner if some user lefts the game.
        /// </summary>
        /// <param name="leftParticipantId"> The id of user who has left.
        /// If it is an empty string, consider that we have left. </param>
        private void ProcessPlayerLeft(string leftParticipantId)
        {
            if (myId == leftParticipantId || leftParticipantId == "")
            {
                //We have left, so we loose.
                currentUserResultType = UserResultType.LOSE;
            }
            else
            {
                //Another user left, so we win.
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
                controllerManager.SecondController.ArmyFinishedMove -= OnOpponentArmyFinishedMove;
            }
            else
            {
                controllerManager.FirstController.ArmyFinishedMove -= OnOpponentArmyFinishedMove;
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