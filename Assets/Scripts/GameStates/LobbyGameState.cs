using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// State of searching for opponents and crating a room.
    /// </summary>
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu lobbyMenu;
        
        //TODO: move it to lobbyMenu?
        [SerializeField] private Text lobbyText;
        [SerializeField] [TextArea] private string authenticationString;
        [SerializeField] [TextArea] private string opponentDisconnectedString;
        [SerializeField] [TextArea] private string unknownErrorString;
        [SerializeField] [TextArea] private string authenticationFailString;
        [SerializeField] [TextArea] private string searchingForOpponentString;
        [SerializeField] [TextArea] private string foundOpponentString;
        
        [SerializeField] private StateManager stateManager;
        [SerializeField] private ExitListener exitListener;
        private MultiplayerController multiplayerController;

        private bool waiting;

        private const byte pingMessageByte = (byte) 'P';
        /// <summary>
        /// If a ping message from the opponent is not received during this time,
        /// display an error.
        /// </summary>
        private const int maxTimeoutSeconds = 15;
        /// <summary>
        /// Time in seconds between two ping messages.
        /// </summary>
        private const int pingDeltaTimeSeconds = 1;

        /// <summary>
        /// To initialize play game state.
        /// </summary>
        public BoardType ConfigurationType { get; set; }

        /// <summary>
        /// Subscribe on all necessary events, risen by network.
        /// </summary>
        private void SubscribeOnNetworkEvents()
        {
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnRoomSetupCompleted += OnOpponentFound;
            multiplayerController.OnRoomSetupError += DisplayRoomSetupError;
            multiplayerController.OnOpponentDisconnected += DisplayOpponentDisconnected;
            multiplayerController.OnMessageReceived += ProcessPingMessage;
            multiplayerController.OnAuthenticated += DisplayAfterAuthenticatedMessage;
            multiplayerController.OnAuthenticationError += DisplayAuthenticationError;
        }
        
        /// <summary>
        /// Unsubscribe from all necessary events, risen by network.
        /// To prevent calling methods several times.
        /// </summary>
        private void UnsubscribeFromNetworkEvents()
        {
            multiplayerController.OnRoomSetupCompleted -= OnOpponentFound;
            multiplayerController.OnRoomSetupError -= DisplayRoomSetupError;
            multiplayerController.OnOpponentDisconnected -= DisplayOpponentDisconnected;
            multiplayerController.OnMessageReceived -= ProcessPingMessage;
            multiplayerController.OnAuthenticated -= DisplayAfterAuthenticatedMessage;
            multiplayerController.OnAuthenticationError -= DisplayAuthenticationError;
        }

        /// <summary>
        /// Starts authentication and opponent searching process.
        /// </summary>
        public void InvokeState()
        {
            multiplayerController = MultiplayerController.GetInstance();
            lobbyText.text = authenticationString;
            menuActivator.OpenMenu(lobbyMenu);            
            SubscribeOnNetworkEvents();

            stateManager.NetworkPlayGameState.ConfigurationType = ConfigurationType;
            if (ConfigurationType == BoardType.SMALL)
            {
                multiplayerController.SignInAndStartMPGame(0);
            }
            else if (ConfigurationType == BoardType.LARGE)
            {
                multiplayerController.SignInAndStartMPGame(1);
            }
            
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
        }

        /// <summary>
        /// We receive ping message so both opponents are ready to play.
        /// If it is not a ping message, ignore it.
        /// </summary>
        private void ProcessPingMessage(byte[] data)
        {
            if (data[0] != pingMessageByte)
            {
                return;
            }
            waiting = false;
            StopCoroutine(WaitForOpponent());
            ChangeToNetworkState();
        }

        private void DisplayOpponentDisconnected()
        {
            multiplayerController.LeaveRoom();
            stateManager.InfoGameState.SetInfoText(opponentDisconnectedString);
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayRoomSetupError()
        {
            multiplayerController.LeaveRoom();
            stateManager.InfoGameState.SetInfoText(unknownErrorString);
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayAuthenticationError()
        {
            multiplayerController.LeaveRoom();
            stateManager.InfoGameState.SetInfoText(authenticationFailString);
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayAfterAuthenticatedMessage()
        {
            lobbyText.text = searchingForOpponentString;
        }
        
        private void OnOpponentFound()
        {
            exitListener.Disable();
            lobbyText.text = foundOpponentString;
            StartCoroutine(WaitForOpponent());
        }

        /// <summary>
        /// Pings the opponent every second until the max timeout.
        /// Solves the problem of loosing the very first message even between connected participants.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForOpponent()
        {
            waiting = true;
            var secondsPassed = 0;
            while (true)
            {
                if (!waiting)
                {
                    yield break;
                }
                var message = new[] {pingMessageByte};
                multiplayerController.SendMessage(message);
                secondsPassed++;
                if (secondsPassed > maxTimeoutSeconds)
                {
                    DisplayRoomSetupError();
                    yield break;
                }
                yield return new WaitForSeconds(pingDeltaTimeSeconds);
            }
        }

        private void ChangeToNetworkState()
        {
            stateManager.ChangeState(StateType.NETWORK_GAME_STATE);
        }

        private void OnExit()
        {
            multiplayerController.LeaveRoom();
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Stops pinging opponent if necessary.
        /// </summary>
        public void CloseState()
        {
            UnsubscribeFromNetworkEvents();
            waiting = false;
            StopCoroutine(WaitForOpponent());
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
        }
    }
}