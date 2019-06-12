using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

namespace Assets.Scripts
{
    // Let them be here.
    public delegate void VoidHandler();
    public delegate void ByteArrayHandler(byte[] data);
    public delegate void StringHandler(string data);
    
    /// <summary>
    /// Class for interaction with Google Play Services.
    /// Provides a set of events to subscribe.
    /// Room is created for 2 players.
    /// Do not forget to call LeaveRoom when finish!
    /// </summary>
    public class MultiplayerController : RealTimeMultiplayerListener
    {
        private static MultiplayerController instance;

        private const uint minimumOpponents = 1;
        private const uint maximumOpponents = 1;
        public event ByteArrayHandler OnMessageReceived;
        public event VoidHandler OnRoomSetupCompleted;
        public event VoidHandler OnRoomSetupError;
        public event VoidHandler OnAuthenticated;

        public event VoidHandler OnAuthenticationError;

        public event VoidHandler OnOpponentDisconnected;
        public event StringHandler OnPlayerLeft;

        public static MultiplayerController GetInstance()
        {
            return instance ?? (instance = new MultiplayerController());
        }

        private MultiplayerController()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        public void SignInAndStartMPGame(uint variation)
        {
            Social.localUser.Authenticate(success =>
            {
                if (!success)
                {
                    OnAuthenticationError?.Invoke();
                }
                else
                {
                    OnAuthenticated?.Invoke();
                    StartMatchMaking(variation);
                }
            });
        }

        private void StartMatchMaking(uint gameVariation) 
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, 
                gameVariation, this);
        }

        public void OnRoomSetupProgress(float percent)
        {
        }
        
        public void OnRoomConnected(bool success)
        {
            if (success) 
            {
                OnRoomSetupCompleted?.Invoke();
            } 
            else
            {
                OnRoomSetupError?.Invoke();
            }
        }

        public void OnLeftRoom()
        {           
            //Because we do not know our id anymore.
            OnPlayerLeft?.Invoke("-1");
            OnOpponentDisconnected?.Invoke();
        }
        
        public void OnParticipantLeft(Participant participant)
        {
            OnOpponentDisconnected?.Invoke();
        }

        public void OnPeersConnected(string[] participantIds)
        {
        }
        
        public void OnPeersDisconnected(string[] participantIds)
        {
            foreach (var participantId in participantIds)
            {
                OnPlayerLeft?.Invoke(participantId);   
            }
        }
        
        public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
        {
            OnMessageReceived?.Invoke(data);
        }

       public List<Participant> GetAllPlayers() {
           return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
       }
        
        public string GetMyParticipantId() {
           return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
        }
        
        /// <summary>
        /// Sends the given message to all in the current room reliably.
        /// </summary>
        public void SendMessage(byte[] message) {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, message);
        }
        public void LeaveRoom()
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
    }
}