using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// A state of tutorial menu.
    /// All ui elements are driven by the canvas the menu attached to.
    /// </summary>
    public class TutorialMenuGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu tutorialMenu;
        [SerializeField] private ExitListener exitListener;
                
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            menuActivator.OpenMenu(tutorialMenu);
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
        }
        
        private void OnExit()
        {
            StateManager.Instance.ChangeState(StateType.START_GAME_STATE);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void CloseState()
        {
            menuActivator.CloseMenu();
            exitListener.Disable();
            exitListener.OnExitClicked -= OnExit;
        }
    }
}