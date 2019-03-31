using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public delegate void ByteArrayHandler(byte[] data);
    public class MultiplayerController : RealTimeMultiplayerListener
    {
        private static MultiplayerController instance;

        private uint minimumOpponents = 1;
        private uint maximumOpponents = 1;
        private uint gameVariation = 0;
        
        private MultiplayerController()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        public static MultiplayerController GetInstance()
        {
            if (instance == null)
            {
                instance = new MultiplayerController();
            }

            return instance;
        }
       
        public void SignInAndStartMPGame()
        {
            Social.localUser.Authenticate((bool success) =>
            {
                ShowMPStatus(success.ToString());
                if (!success)
                {
                    SignInAndStartMPGame();
                }
                else
                {
                    StartMatchMaking();
                }
            });
        }
        
        public void TrySilentSignIn() 
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated) 
            {
                PlayGamesPlatform.Instance.Authenticate((bool success) =>
                {
                    if (success) 
                    {
                        Debug.Log("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                    } 
                    else 
                    {
                        Debug.Log("We're not signed in.");
                    }
                }, true);
            }
            else 
            {
                Debug.Log("We're already signed in");
            }
        }
       
        private void StartMatchMaking() 
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
        }

        public void OnRoomSetupProgress(float percent)
        {
            ShowMPStatus("We are " + percent + "% done with setup");
        }

        public event VoidHandler OnRoomSetupCompleted;
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
            }
        }

        public void OnLeftRoom()
        {
            ShowMPStatus("We have left the room.");
        }

        public void OnParticipantLeft(Participant participant)
        {
            throw new System.NotImplementedException();
        }

        public void OnPeersConnected(string[] participantIds)
        {
            foreach (string participantID in participantIds)
            {
                ShowMPStatus("Player " + participantID + " has joined.");
            }
        }

        public void OnPeersDisconnected(string[] participantIds)
        {
            foreach (string participantID in participantIds)
            {
                ShowMPStatus("Player " + participantID + " has left.");
            }
        }

        public event ByteArrayHandler OnMessageReceived;
        public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
        {
            //ShowMPStatus("Received some gameplay messages from participant ID: " + senderId);
            OnMessageReceived?.Invoke(data);
        }

       public List<Participant> GetAllPlayers() {
           return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
       }
        
        public string GetMyParticipantId() {
           return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
        }
        
        public void SendMessage(byte[] message) {
            //Debug.Log ("Sending my update message  " + message + " to all players in the room");
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