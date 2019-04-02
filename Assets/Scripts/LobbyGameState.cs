using UnityEngine;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.GetInstance();
        public LobbyMenu lobbyMenu;
        public StateManager stateManager;
        private MultiplayerController MultiplayerController;

        public void InvokeState()
        {
            menuActivator.OpenMenu(lobbyMenu);
            MultiplayerController = MultiplayerController.GetInstance();
            
            MultiplayerController.logText = lobbyMenu.logText;
            MultiplayerController.logText.text = "";
            
            MultiplayerController.OnRoomSetupCompleted += ChangeStateToNetworkGameState;
            MultiplayerController.SignInAndStartMPGame();
        }

        private void ChangeStateToNetworkGameState()
        {
            stateManager.ChangeState(StateType.NETWORK_GAME_STATE);
        }
        
        public void CloseState()
        {
            menuActivator.CloseMenu();
            MultiplayerController.OnRoomSetupCompleted -= ChangeStateToNetworkGameState;
        }
    }
}