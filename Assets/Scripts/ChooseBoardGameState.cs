using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseBoardGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.GetInstance();
        public SimpleMenu chooseBoardMenu;
        public ExitListener exitListener;

        public StateType NextStateType { get; set; }
        public void InvokeState()
        {
            exitListener.Enable();
            exitListener.OnExitClicked += MoveToStartMenu;
            menuActivator.OpenMenu(chooseBoardMenu);
        }

        private void MoveToStartMenu()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
            exitListener.OnExitClicked -= MoveToStartMenu;
            exitListener.Disable();
        }
    }
}