using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.UI;

namespace Assets.Scripts
{
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
            menuActivator.OpenMenu(playMenu);

            SetupListener();

            multiplayerController = MultiplayerController.GetInstance();
            allPlayers = multiplayerController.GetAllPlayers();
            string hostID = ChooseHost();
            string myId = multiplayerController.GetMyParticipantId();

            multiplayerController.logText = logText;

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

        private void SetupListener()
        {
            inputListener.Init(board.width, board.height);
            boardCreator.patternButton.GetComponent<BoardButton>().inputListener = inputListener;
            playMenu.startButton.GetComponent<StartButton>().inputListener = inputListener;
        }
        
        private string ChooseHost()
        {
            return allPlayers.Min(participant => participant.ParticipantId);
        }

        private void SetupRoundHost()
        {
            storage.Reset();
            boardCreator.FillBoardStorageRandomly();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST, 
                boardCreator.FirstArmy, storage, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, 
                boardCreator.SecondArmy, storage, this);

            List<byte> bytes = boardCreator.ConvertBoardStorageToBytes();
            
            //Insert 'S' -- Setup message.
            bytes.Insert(0, (byte) 'S');
            //Host is always First
            bytes.Add(2);

            logText.text += "Sending setup info...\n";
            
            multiplayerController.SendMessage(bytes.ToArray());
        }
        
        private void SetupRoundFromNetwork()
        {
            byte[] message = multiplayerController.lastMessage;
            if (message[0] != 'S')
            {
                return;
            }

            logText.text += "Create board from network..." + "\n";
            multiplayerController.OnMessageReceived -= SetupRoundFromNetwork;
            storage.Reset();
            
            boardCreator.FillBoardStorageFromArray(message.Skip(1).ToArray());
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST, 
                boardCreator.FirstArmy, storage, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, 
                boardCreator.SecondArmy, storage, this);
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
    }
}