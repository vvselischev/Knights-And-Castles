using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// State for displaying messages.
    /// Text object for message must be set in the editor.
    /// </summary>
    public class InfoGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu menu;
        [SerializeField] private Text infoText;
        [SerializeField] private ExitListener exitListener;
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            menuActivator.OpenMenu(menu);
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
        }

        /// <summary>
        /// Method is called by the exit listener when exit button is pressed.
        /// </summary>
        private void OnExit()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }
        
        /// <summary>
        /// Displays the given text.
        /// </summary>
        public void SetInfoText(string text)
        {
            infoText.text = text;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void CloseState()
        {
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
        }
    }
}