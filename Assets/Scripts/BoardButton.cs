using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a button on board.
    /// Notifies InputListener on click if active.
    /// </summary>
    public class BoardButton : MonoBehaviour
    {
        private int boardX;
        private int boardY;

        [SerializeField] private GameIcon frame;

        public InputListener InputListener { get; set; }

        /// <summary>
        /// This position will be transferred to the InputListener when pressed.
        /// </summary>
        public void Initialize(int x, int y)
        {
            boardX = x;
            boardY = y;
        }

        public void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessBoardClick(boardX, boardY);
            }
        }

        /// <summary>
        /// Enables button. Clicks are processed.
        /// </summary>
        public void Enable()
        {
            enabled = true;
        }

        /// <summary>
        /// Enables button. Clicks are not processed.
        /// </summary>
        public void Disable()
        {
            DisableFrame();
            enabled = false;
        }
        
        /// <summary>
        /// Disables frame around the button.
        /// </summary>
        public void DisableFrame()
        {
            frame.Disable();
        }

        /// <summary>
        /// Enables frame around the button.
        /// </summary>
        public void EnableFrame()
        {
            frame.Enable();
        }
    }
}