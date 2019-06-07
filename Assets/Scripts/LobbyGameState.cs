using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu lobbyMenu;
        
        //TODO: move it to lobbyMenu
        [SerializeField] private Text lobbyText;
        
        [SerializeField] private StateManager stateManager;
        [SerializeField] private ExitListener exitListener;
        private MultiplayerController multiplayerController;

        public BoardType ConfigurationType { get; set; }

        public void InvokeState()
        {
            lobbyText.text = "Searching for opponent...";
            menuActivator.OpenMenu(lobbyMenu);
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnRoomSetupCompleted += OnOpponentFound;
            multiplayerController.OnRoomSetupError += DisplayRoomSetupError;
            multiplayerController.OnOpponentDisconnected += DisplayOpponentDisconnected;
            
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
            StartCoroutine(PingOpponent());
        }

        private IEnumerator PingOpponent()
        {
            const int pings = 50;
            const float between = 0.1f;
            for (int i = 0; i < pings; i++)
            {
                yield return new WaitForSeconds(between);
            }
            ChangeToNetworkState();
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
        }
    }
}