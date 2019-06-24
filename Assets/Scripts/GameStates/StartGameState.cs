using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents the state of the main menu in the game.
    /// Simply activates and deactivates the main menu.
    /// </summary>
    public class StartGameState : MonoBehaviour, IGameState
    {
        private MenuActivator menuActivator = MenuActivator.Instance;
        [SerializeField] private SimpleMenu startMenu;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void InvokeState()
        {
            menuActivator.OpenMenu(startMenu);
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