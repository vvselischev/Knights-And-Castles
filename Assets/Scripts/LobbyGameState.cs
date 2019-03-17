using UnityEngine;

namespace Assets.Scripts
{
    public class LobbyGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.GetInstance();
        public LobbyMenu lobbyMenu;
        public StateManager stateManager;
        public NetworkPlayGameState networkPlayGameState;
        private MultiplayerController MultiplayerController;

        public void InvokeState()
        {
            menuActivator.OpenMenu(lobbyMenu);
            MultiplayerController = MultiplayerController.GetInstance();
            MultiplayerController.logText = lobbyMenu.logText;
            MultiplayerController.OnRoomSetupCompleted += () =>
            {
                stateManager.ChangeState(StateType.NETWORK_GAME_STATE);
            };
//            MultiplayerController.SignInAndStartMPGame();
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
        }
    }
}