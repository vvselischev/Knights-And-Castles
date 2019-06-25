using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// A state of tutorial menu.
    /// All ui elements are driven by the canvas the menu attached to.
    /// </summary>
    public class TutorialGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu tutorialMenu;
                
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            menuActivator.OpenMenu(tutorialMenu);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void CloseState()
        {
            menuActivator.CloseMenu();
        }
    }
}