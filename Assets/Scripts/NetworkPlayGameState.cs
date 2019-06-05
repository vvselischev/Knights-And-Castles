using System;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO: should we disable exit listener on setup?
    public class NetworkPlayGameState : PlayGameState
    {
        [SerializeField] private NetworkInputListener networkInputListener;
        private MultiplayerController multiplayerController;
        private List<Participant> allPlayers;
        private bool isHost;
        private TurnType myTurnType;
        private UserResultType currentUserResultType;

        [SerializeField] private Text logText;
        private string myId;

        public override void InvokeState()
        {
            inputListener = networkInputListener;
            logText.text += "Invoking... ";
            SetupGame();
            
            logText.text += "Finishing setup... ";
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.logText = logText;
            multiplayerController.OnPlayerLeft += ProcessPlayerLeft;

            string hostId;
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
            logText.text += "Creating board...";
            boardFactory.FillBoardStorageRandomly(boardStorage);

            logText.text += "Board created. Converting to bytes...";
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
            
            logText.text += "Create board from network..." + "\n";

            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray(), boardStorage);
            FinishSetup();
                
            
            logText.text += "Finish setup listeners";
            InitNewRound();
            
            //Because host is the first to move
            boardStorage.InvertBoard();
            playMenu.DisableUI();
        }

        private void FinishSetup()
        {
            board.SetInputListener(networkInputListener);
            var firstController = new UserController(PlayerType.FIRST, boardStorage, boardFactory, this, armyText);
            var secondController = new UserController(PlayerType.SECOND, boardStorage, boardFactory, this, armyText);
            controllerManager = new ControllerManager(firstController, secondController);
            networkInputListener.Initialize(controllerManager, board.Width, board.Height);
            playMenu.Initialize(boardManager, inputListener);
            turnManager.Initialize(boardManager, controllerManager);
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

        private void ProcessPlayerLeft(string message)
        {
            logText.text += message + "\n";
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
            
            multiplayerController.LeaveRoom();
            
            logText.text = "Result: " + resultType + '\n';
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
            stateManager.resultGameState.Initialize(currentUserResultType, playMode);
            stateManager.ChangeState(StateType.RESULT_GAME_STATE);
        }

        protected override void ExitGame()
        {
            multiplayerController.LeaveRoom();
            //Further ProcessPlayerLeft will be invoked.
        }

        public override void CloseState()
        {
            multiplayerController.OnPlayerLeft -= ProcessPlayerLeft;
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