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
        
        //TODO: move it to lobbyMenu
        [SerializeField] private Text lobbyText;
        
        [SerializeField] private StateManager stateManager;
        [SerializeField] private ExitListener exitListener;
        private MultiplayerController multiplayerController;

        /// <summary>
        /// If a ping message from the opponent is not received during this time,
        /// display an error.
        /// </summary>
        private const int MAX_TIMEOUT_SECONDS = 15;

        public BoardType ConfigurationType { get; set; }

        public void InvokeState()
        {
            lobbyText.text = "Searching for opponent...";
            menuActivator.OpenMenu(lobbyMenu);
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnRoomSetupCompleted += OnOpponentFound;
            multiplayerController.OnRoomSetupError += DisplayRoomSetupError;
            multiplayerController.OnOpponentDisconnected += DisplayOpponentDisconnected;
            multiplayerController.OnMessageReceived += ProcessMessage;
            
            (stateManager.GetState(StateType.NETWORK_GAME_STATE) as PlayGameState).ConfigurationType = ConfigurationType;

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

        private void ProcessMessage(byte[] data)
        {
            if (data[0] == (byte) 'P')
            {  
                StopCoroutine(WaitForOpponent());
                ChangeToNetworkState();  
            }
        }

        private void DisplayOpponentDisconnected()
        {
            multiplayerController.LeaveRoom();
            stateManager.infoGameState.SetInfoText("Opponent disconnected!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void DisplayRoomSetupError()
        {
            multiplayerController.LeaveRoom();
            stateManager.infoGameState.SetInfoText("Something went wrong!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void OnOpponentFound()
        {
            lobbyText.text = "Opponent found!";
            StartCoroutine(WaitForOpponent());
        }

        /// <summary>
        /// Pings the opponent.
        /// 'P' -- ping message.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForOpponent()
        {
            var secondsPassed = 0;
            while (true)
            {
                var message = new[] {(byte)'P'};
                multiplayerController.SendMessage(message);
                secondsPassed++;
                if (secondsPassed > MAX_TIMEOUT_SECONDS)
                {
                    DisplayRoomSetupError();
                    yield break;
                }
                yield return new WaitForSeconds(1);
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
        
        public void CloseState()
        {
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
            multiplayerController.OnRoomSetupCompleted -= OnOpponentFound;
            multiplayerController.OnOpponentDisconnected -= DisplayOpponentDisconnected;
            multiplayerController.OnRoomSetupError -= DisplayRoomSetupError;
            multiplayerController.OnMessageReceived -= ProcessMessage;
        }
    }
}