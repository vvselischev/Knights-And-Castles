using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Simple icon. Can be changed in color and enabled/disabled.
    /// </summary>
    public class GameIcon : MonoBehaviour
    {
        private Sprite defaultSprite;
        private Image currentImage;
        
        /// <summary>
        /// Color of the icon when it is enabled.
        /// </summary>
        private Color enabledColor = Color.yellow;

        /// <summary>
        /// Determines, whether this icon should be enabled after reset.
        /// </summary>
        [SerializeField] private bool enabledByDefault = true;

        /// <summary>
        /// Returns to the default state.
        /// </summary>
        public void Reset()
        {
            currentImage.enabled = enabledByDefault;
            currentImage.sprite = defaultSprite;
        }
        
        /// <summary>
        /// Color the image with the given color.
        /// </summary>
        public void ChangeColor(Color color)
        {
            currentImage.enabled = true;
            currentImage.color = color;
        }

        /// <summary>
        /// Disables the icon.
        /// </summary>
        public void Disable()
        {
            Reset();
            currentImage.enabled = false;
        }
        
        /// <summary>
        /// Enables the icon.
        /// </summary>
        public void Enable()
        {
            currentImage.enabled = true;
            currentImage.color = enabledColor;
        }

        /// <summary>
        /// Save current state (set in the editor) as defult.
        /// </summary>
        public void Awake()
        {
            defaultSprite = GetComponent<Image>().sprite;
            currentImage = GetComponent<Image>();
            enabledByDefault = currentImage.enabled;
        }
    }
}