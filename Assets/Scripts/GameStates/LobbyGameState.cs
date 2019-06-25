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
        /// To initialize play state.
        /// </summary>
        public BoardType ConfigurationType { get; set; }

        private void Awake()
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
        /// Starts authentication and opponent searching process.
        /// </summary>
        public void InvokeState()
        {
            lobbyText.text = "Authentication...";
            menuActivator.OpenMenu(lobbyMenu);            
            
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
            stateManager.InfoGameState.SetInfoText("Opponent disconnected!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayRoomSetupError()
        {
            multiplayerController.LeaveRoom();
            stateManager.InfoGameState.SetInfoText("Something went wrong!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayAuthenticationError()
        {
            multiplayerController.LeaveRoom();
            stateManager.InfoGameState.SetInfoText("Authentication failed!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayAfterAuthenticatedMessage()
        {
            lobbyText.text = "Searching for opponent...";
        }
        
        private void OnOpponentFound()
        {
            exitListener.Disable();
            lobbyText.text = "Opponent found!";
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
            waiting = false;
            StopCoroutine(WaitForOpponent());
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
        }
    }
}