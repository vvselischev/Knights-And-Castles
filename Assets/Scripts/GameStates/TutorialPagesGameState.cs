using UnityEngine;

namespace Assets.Scripts
{
    public class TutorialPagesGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu tutorialPagesMenu;
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            menuActivator.OpenMenu(tutorialPagesMenu);
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