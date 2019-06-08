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
        [SerializeField] private SimpleMenu startMenu;
        [SerializeField] private Text infoText;
        [SerializeField] private ExitListener exitListener;
        public void InvokeState()
        {
            menuActivator.OpenMenu(startMenu);
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
        }

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

        public void CloseState()
        {
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
        }
    }
}