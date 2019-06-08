using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a text, displaying army composition in play mode.
    /// Text game object must be set in the editor.
    /// </summary>
    public class ArmyText : MonoBehaviour
    {
        [SerializeField] 
        private Text armyCompositionText;

        public void Init()
        {
            Clear();
            Enable();
        }

        /// <summary>
        /// Displays a given army composition.
        /// </summary>
        /// <param name="army"></param>
        public void UpdateText(Army army)
        {
            if (army == null)
            {
                Clear();
            }
            else
            {
                armyCompositionText.text = army.ArmyComposition.ToString();
            }
        }

        public void Clear()
        {
            armyCompositionText.text = "";
        }

        /// <summary>
        /// Enables the text. It is displayed on screen.
        /// </summary>
        public void Enable()
        {
            armyCompositionText.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Disables the text. It is not displayed on screen.
        /// </summary>
        public void Disable()
        {
            armyCompositionText.gameObject.SetActive(false);
        }
    }
}
