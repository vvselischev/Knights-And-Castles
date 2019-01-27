using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkPlayGameState : PlayGameState
    {
        private MultiplayerController multiplayerController;
        private List<Participant> allPlayers;
        private bool isHost;

        public Text logText;
        public override void InvokeState()
        {
            menuActivator.OpenMenu(playMenu);
            
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
                InitNewGame();
            }
            else
            {
                logText.text += "Waiting for round data..." + "\n";
                multiplayerController.OnMessageReceived += SetupRoundFromNetwork;
            }
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
            foreach (byte b in bytes)
            {
                logText.text += b + " ";
            }

            logText.text += "\n";
            
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
            
            foreach (byte b in message)
            {
                logText.text += b + " ";
            }

            logText.text += "\n";
            
            boardCreator.FillBoardStorageFromArray(message.Skip(1).ToArray());
            
            //Because we are not host and our type is Second
            storage.InvertBoard();
            controllerManager.FirstController = new UserController(PlayerType.FIRST, 
                boardCreator.FirstArmy, storage, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, 
                boardCreator.SecondArmy, storage, this);
            InitNewGame();
        }
        
        protected override TurnType GetFirstTurn()
        {
            //Host makes first turn (as PlayerType.First).
            if (isHost)
            {
                return TurnType.FIRST;
            }
            return TurnType.SECOND;
        }
    }
}