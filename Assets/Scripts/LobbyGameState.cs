using UnityEngine;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] protected SimpleMenu lobbyMenu;
        [SerializeField] private StateManager stateManager;
        [SerializeField] protected ExitListener exitListener;
        private MultiplayerController MultiplayerController;

        public void InvokeState()
        {
            menuActivator.OpenMenu(lobbyMenu);
            MultiplayerController = MultiplayerController.GetInstance();
            MultiplayerController.OnRoomSetupCompleted += ChangeStateToNetworkGameState;
            
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
            
            MultiplayerController.SignInAndStartMPGame();
        }

        private void ChangeStateToNetworkGameState()
        {
            stateManager.ChangeState(StateType.NETWORK_GAME_STATE);
        }

        private void OnExit()
        {
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }
        
        public void CloseState()
        {
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
            MultiplayerController.OnRoomSetupCompleted -= ChangeStateToNetworkGameState;
        }
    }
}