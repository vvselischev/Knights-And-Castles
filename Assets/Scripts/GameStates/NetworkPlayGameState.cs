using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

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
            boardFactory.FillBoardStorageRandomly(boardStorage);

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

            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray(), boardStorage);
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
            var firstController = new UserController(PlayerType.FIRST, boardStorage, boardFactory, this, armyText);
            var secondController = new UserController(PlayerType.SECOND, boardStorage, boardFactory, this, armyText);

            if (isHost)
            {
                secondController.FinishedMove += OnOpponentFinishedMove;
            }
            else
            {
                firstController.FinishedMove += OnOpponentFinishedMove;
            }
            
            controllerManager = new ControllerManager(firstController, secondController);
            networkInputListener.Initialize(controllerManager, board.Width, board.Height);
            playMenu.Initialize(boardManager, inputListener);
            turnManager.Initialize(boardManager, controllerManager);
        }

        private void OnOpponentFinishedMove()
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
                ProcessPlayerLeft("-1");
            }
        }

        private void ProcessPlayerLeft(string message)
        {
            if (myId == message || message == "-1")
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
        
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            
            if (resultType == ResultType.FIRST_WIN)
            {
                if (isHost)
                {
                    lerpedText.PerformLerpString("You win!", Color.green);
                    currentUserResultType = UserResultType.WIN;
                }
                else
                {
                    lerpedText.PerformLerpString("You lose...", Color.red);
                    currentUserResultType = UserResultType.LOSE;
                }
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                if (!isHost)
                {
                    lerpedText.PerformLerpString("You win!", Color.green);
                    currentUserResultType = UserResultType.WIN;
                    
                }
                else
                {
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

        protected override void CloseGame()
        {
            if (isHost)
            {
                controllerManager.SecondController.FinishedMove -= OnOpponentFinishedMove;
            }
            else
            {
                controllerManager.FirstController.FinishedMove -= OnOpponentFinishedMove;
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

        public override void CloseState()
        {
            networkInputListener.Stop();
            base.CloseState();
        }

        protected override void InitNewRound()
        {
            base.InitNewRound();
            
            if ((turnManager.CurrentTurn == TurnType.FIRST) == isHost)
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