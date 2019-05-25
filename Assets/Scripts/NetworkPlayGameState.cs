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
        public NetworkInputListener networkInputListener;
        private MultiplayerController multiplayerController;
        private List<Participant> allPlayers;
        private bool isHost;
        private TurnType myTurnType;

        public Text logText;

        public override void InvokeState()
        { 
            SetupGame();
            
            logText.text = "";
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.logText = logText;
            
            logText.text += "Invoking\n";
            SetupListeners();
            logText.text += "Finish setup\n";
            
            allPlayers = multiplayerController.GetAllPlayers();
            
            string hostID = ChooseHost();
            string myId = multiplayerController.GetMyParticipantId();
            
            isHost = false;
            
            //Let the host set up the round.
            if (hostID == myId)
            {
                isHost = true;
                SetupRoundHost();
                myTurnType = TurnType.FIRST;
                InitNewGame();
            }
            else
            {
                myTurnType = TurnType.SECOND;
                multiplayerController.OnMessageReceived += SetupRoundFromNetwork;
            }
        }

        private void SetupListeners()
        {
            networkInputListener.Init();
            board.SetInputListener(networkInputListener);
            playMenu.finishTurnButton.GetComponent<FinishTurnButton>().inputListener = networkInputListener;
            playMenu.splitButton.GetComponent<SplitButton>().inputListener = networkInputListener;
        }
        
        private string ChooseHost()
        {
            return allPlayers.Min(participant => participant.ParticipantId);
        }

        private void SetupRoundHost()
        {
            logText.text += "Creating board...";
            boardFactory.FillBoardStorageRandomly(storage);

            logText.text += "Board created. Converting to bytes...";
            boardFactory.logText = logText;
            List<byte> message = boardFactory.ConvertBoardStorageToBytes(storage);
            
            //Insert 'S' -- Setup message.
            message.Insert(0, (byte) 'S');

            multiplayerController.SendMessage(message.ToArray());
            
            controllerManager.firstController = new UserController(PlayerType.FIRST, storage, boardFactory,this, armyText);
            controllerManager.secondController = new UserController(PlayerType.SECOND, storage, boardFactory,this, armyText);
        }
        
        private void SetupRoundFromNetwork(byte[] message)
        {
            if (message[0] != 'S')
            {
                return;
            }
 
            multiplayerController.OnMessageReceived -= SetupRoundFromNetwork;
            
            logText.text += "Create board from network..." + "\n";

            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray(), storage);
            
            controllerManager.firstController = new UserController(PlayerType.FIRST, storage, boardFactory,this, armyText);
            controllerManager.secondController = new UserController(PlayerType.SECOND, storage, boardFactory,this, armyText);
            logText.text += "Finish setup listeners";
            InitNewGame();
            
            //Because host is the first to move
            storage.InvertBoard();
            playMenu.DisableUI();
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
        
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);

            multiplayerController.LeaveRoom();
            
            logText.text = "Result: " + resultType + '\n';
            if (resultType == ResultType.FIRST_WIN)
            {
                if (isHost)
                {
                    uiManager.PerformLerpString("You win!", Color.green);
                }
                else
                {
                    uiManager.PerformLerpString("You lose...", Color.red);
                }
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                if (!isHost)
                {
                    uiManager.PerformLerpString("You win!", Color.green);
                }
                else
                {
                    uiManager.PerformLerpString("You lose...", Color.red);
                }
            }
            else if (resultType == ResultType.DRAW)
            {
                uiManager.PerformLerpString("Draw", Color.blue);
            }
        }

        protected override void ExitGame()
        {
            multiplayerController.LeaveRoom();
            base.ExitGame();
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