using UnityEngine;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.GetInstance();
        public LobbyMenu lobbyMenu;
        public StateManager stateManager;
        public NetworkPlayGameState networkPlayGameState;
        private MultiplayerController multiplayerController;

        public void InvokeState()
        {
            menuActivator.OpenMenu(lobbyMenu);
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.logText = lobbyMenu.logText;
            multiplayerController.OnRoomSetupCompleted += () =>
            {
                stateManager.ChangeState(StateType.NETWORK_GAME_STATE);
            };
            multiplayerController.SignInAndStartMPGame();
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
        }
    }
}