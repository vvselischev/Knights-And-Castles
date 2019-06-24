using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Game state of choosing board mode.
    /// </summary>
    public class ChooseBoardGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu chooseBoardMenu;
        [SerializeField] private ExitListener exitListener;

        /// <summary>
        /// Property to remember the play mode, chosen in StartGameState.
        /// </summary>
        public StateType NextStateType { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            exitListener.Enable();
            exitListener.OnExitClicked += MoveToStartMenu;
            menuActivator.OpenMenu(chooseBoardMenu);
        }

        /// <summary>
        /// Moves to the start menu.
        /// </summary>
        private void MoveToStartMenu()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void CloseState()
        {
            menuActivator.CloseMenu();
            exitListener.OnExitClicked -= MoveToStartMenu;
            exitListener.Disable();
        }
    }
}