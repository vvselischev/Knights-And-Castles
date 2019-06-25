using UnityEngine;

namespace Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Calls split button process method on the listener.
    /// </summary>
    public class SplitButton : LockableButton
    {
        [SerializeField] private GameIcon frame;
        
        public override void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessSplitButtonClick();
            }
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