using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

namespace Assets.Scripts
{
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

        //We have only one opponent in this version.
        private const uint minimumOpponents = 1;
        private const uint maximumOpponents = 1;
        
        /// <summary>
        /// Event is risen when a network message is received.
        /// Contains the message.
        /// </summary>
        public event ByteArrayHandler OnMessageReceived;
        /// <summary>
        /// Event is risen when both opponents are connected to the room.
        /// </summary>
        public event VoidHandler OnRoomSetupCompleted;
        /// <summary>
        /// Event is risen if an error occured during the room setup.
        /// </summary>
        public event VoidHandler OnRoomSetupError;
        /// <summary>
        /// Event is risen after the user is authenticated via google account.
        /// </summary>
        public event VoidHandler OnAuthenticated;
        /// <summary>
        /// Event is risen if an error occured during the authentication process.
        /// </summary>
        public event VoidHandler OnAuthenticationError;
        /// <summary>
        /// An event is risen if an opponent disconnects from the room.
        /// </summary>
        public event VoidHandler OnOpponentDisconnected;
        /// <summary>
        /// An event is risen when the player leaves the room.
        /// Contains leaving player's participant id or an empty string if it is the id of the current player.
        /// </summary>
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

        /// <summary>
        /// Starts the authentication process.
        /// </summary>
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

        /// <summary>
        /// Creates the game and starts searching for opponents.
        /// </summary>
        private void StartMatchMaking(uint gameVariation) 
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, 
                gameVariation, this);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        //This method (strangely) is only called on 20% and rarely on 80%, so we do not want to display it anyhow.
        public void OnRoomSetupProgress(float percent) {}
        
        /// <summary>
        /// Processes the result of the connection to the room.
        /// Rises the corresponding event.
        /// </summary>
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

        /// <summary>
        /// Processes player leaving the room.
        /// </summary>
        public void OnLeftRoom()
        {           
            //Because we do not know our id anymore.
            OnPlayerLeft?.Invoke("-1");
            OnOpponentDisconnected?.Invoke();
        }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void OnParticipantLeft(Participant participant)
        {
            OnOpponentDisconnected?.Invoke();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        //Nothing to do here...
        public void OnPeersConnected(string[] participantIds) {}
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void OnPeersDisconnected(string[] participantIds)
        {
            foreach (var participantId in participantIds)
            {
                OnPlayerLeft?.Invoke(participantId);   
            }
        }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
        {
            OnMessageReceived?.Invoke(data);
        }

        /// <summary>
        /// Returns the list of all connected participants.
        /// </summary>
       public List<Participant> GetAllPlayers() {
           return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
       }
        
        /// <summary>
        /// Returns the current participant's id.
        /// </summary>
        public string GetMyParticipantId() {
           return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
        }
        
        /// <summary>
        /// Sends the given message to all in the current room reliably.
        /// </summary>
        public void SendMessage(byte[] message) {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, message);
        }
        
        /// <summary>
        /// Leaves the room by the current player.
        /// </summary>
        public void LeaveRoom()
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
    }
}