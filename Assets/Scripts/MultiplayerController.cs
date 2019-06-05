using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public delegate void ByteArrayHandler(byte[] data);
    public delegate void StringHandler(string data);
    public class MultiplayerController : RealTimeMultiplayerListener
    {
        private static MultiplayerController instance;

        private const uint minimumOpponents = 1;
        private const uint maximumOpponents = 1;
        public event ByteArrayHandler OnMessageReceived;
        public event VoidHandler OnRoomSetupCompleted;
        public event VoidHandler OnRoomSetupError;
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
                ShowMPStatus("I am + " + Social.localUser.userName);
                ShowMPStatus(success.ToString());
                if (!success)
                {
                    PlayGamesPlatform.Instance.SignOut();
                    OnRoomSetupError?.Invoke();
                }
                else
                {
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
            ShowMPStatus("We are " + percent + "% done with setup");
        }
        
        public void OnRoomConnected(bool success)
        {
            if (success) 
            {
                ShowMPStatus("We are connected to the room! Start our game now.");
                OnRoomSetupCompleted?.Invoke();
            } 
            else
            {
                ShowMPStatus("Encountered some error connecting to the room.");
                OnRoomSetupError?.Invoke();
            }
        }

        public void OnLeftRoom()
        {
            ShowMPStatus("We have left the room. ");
            
            //Because we do not know our id anymore.
            OnPlayerLeft?.Invoke("-1");
            OnRoomSetupError?.Invoke();
        }
        
        public void OnParticipantLeft(Participant participant)
        {
            ShowMPStatus("Participant left.");
            OnRoomSetupError?.Invoke();
        }

        public void OnPeersConnected(string[] participantIds)
        {
            foreach (var participantId in participantIds)
            {
                ShowMPStatus("Player " + participantId + " has joined.");
            }
        }
        
        public void OnPeersDisconnected(string[] participantIds)
        {
            foreach (var participantId in participantIds)
            {
                ShowMPStatus("Player " + participantId + " has left.");
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
        
        public void SendMessage(byte[] message) {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, message);
        }

        public Text logText;
        private void ShowMPStatus(string message)
        {
            Debug.Log(message);
            logText.text += "\n" + message;
        }

        public void LeaveRoom()
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }
    }
}