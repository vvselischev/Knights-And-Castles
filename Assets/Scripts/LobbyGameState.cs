using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu lobbyMenu;
        [SerializeField] private StateManager stateManager;
        [SerializeField] private ExitListener exitListener;
        private MultiplayerController multiplayerController;

        [SerializeField] private Text lobbyLogText;
        public BoardType ConfigurationType { get; set; }

        public void InvokeState()
        {
            menuActivator.OpenMenu(lobbyMenu);
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnRoomSetupCompleted += ChangeStateToNetworkGameState;
            multiplayerController.OnRoomSetupError += DisplayError;
            multiplayerController.logText = lobbyLogText;

            lobbyLogText.text = "Logging... ";
            
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

        private void DisplayError()
        {
            multiplayerController.LeaveRoom();
            stateManager.infoGameState.SetInfoText("Opponent disconnected!\nPlease, try again.");
            stateManager.ChangeState(StateType.INFO_GAME_STATE);
        }

        private void ChangeStateToNetworkGameState()
        {
            lobbyLogText.text = "Changing to network... ";
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
            multiplayerController.OnRoomSetupCompleted -= ChangeStateToNetworkGameState;
            multiplayerController.OnRoomSetupError -= DisplayError;
            lobbyLogText.text = "Closing... ";
        }
    }
}