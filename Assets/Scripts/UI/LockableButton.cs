using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Simple button. Can be locked.
    /// When locked, color is changed to red and clicks are not processed.
    /// </summary>
    public class LockableButton : MonoBehaviour
    {
        /// <summary>
        /// The icon of the button.
        /// </summary>
        [SerializeField] private GameIcon icon;
        /// <summary>
        /// The default color, when the button is not locked.
        /// </summary>
        [SerializeField] private Color defaultColor = Color.white;
        /// <summary>
        /// The color when the button is locked.
        /// </summary>
        [SerializeField] private Color lockColor = Color.red;

        /// <summary>
        /// The current input listener to process clicks.
        /// </summary>
        public InputListener InputListener { get; set; }

        /// <summary>
        /// Method is assumed to be specified in child classes.
        /// </summary>
        public virtual void OnClick() {}
        
        /// <summary>
        /// Disables the button and colors it to the lock color.
        /// </summary>
        public void Lock()
        {
            enabled = false;
            icon.ChangeColor(lockColor);
        }

        /// <summary>
        /// Enables the button and marks it to the default color.
        /// </summary>
        public void Unlock()
        {
            enabled = true;
            icon.ChangeColor(defaultColor);
        }
    }
}