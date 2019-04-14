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
        public NetworkInputListener inputListener;
        private MultiplayerController multiplayerController;
        private List<Participant> allPlayers;
        private bool isHost;
        private TurnType myTurnType;

        public Text logText;

        public override void InvokeState()
        {
            SetupGame();
            SetupListeners();
            
            multiplayerController = MultiplayerController.GetInstance();
            
            allPlayers = multiplayerController.GetAllPlayers();
            
            string hostID = ChooseHost();
            string myId = multiplayerController.GetMyParticipantId();

            multiplayerController.logText = logText;
            logText.text = "";
            
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
            inputListener.Init(storage.GetBoardWidth(), storage.GetBoardHeight());
            boardFactory.patternButton.GetComponent<BoardButton>().inputListener = inputListener;
            playMenu.startButton.GetComponent<StartButton>().inputListener = inputListener;
        }
        
        private string ChooseHost()
        {
            return allPlayers.Min(participant => participant.ParticipantId);
        }

        private void SetupRoundHost()
        {
            boardFactory.FillBoardStorageRandomly();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST, storage, boardFactory,this, armyText);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, storage, boardFactory,this, armyText);

            List<byte> bytes = boardFactory.ConvertBoardStorageToBytes();
            
            //Insert 'S' -- Setup message.
            bytes.Insert(0, (byte) 'S');
            //Host is always First
            bytes.Add(2);

            logText.text += "Sending setup info...\n";
            
            multiplayerController.SendMessage(bytes.ToArray());
        }
        
        private void SetupRoundFromNetwork(byte[] message)
        {
            if (message[0] != 'S')
            {
                return;
            }

            logText.text += "Create board from network..." + "\n";
            multiplayerController.OnMessageReceived -= SetupRoundFromNetwork;

            boardFactory.FillBoardStorageFromArray(message.Skip(1).ToArray());
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST, storage, boardFactory,this, armyText);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, storage, boardFactory,this, armyText);
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
            inputListener.Stop();
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