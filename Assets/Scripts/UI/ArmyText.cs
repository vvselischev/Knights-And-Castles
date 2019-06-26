using System;
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
        /// <summary>
        /// The text object.
        /// </summary>
        [SerializeField] private Text armyCompositionText;

        [SerializeField] [TextArea] private string selectArmyRequestString;
        [SerializeField] [TextArea] private string exceedingArmyLimitString;

        /// <summary>
        /// Clears the text and displays it.
        /// </summary>
        public void Init()
        {
            Clear();
            Enable();
        }

        /// <summary>
        /// Displays a given army composition.
        /// </summary>
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

        /// <summary>
        /// Replaces the text with an empty string.
        /// </summary>
        public void Clear()
        {
            armyCompositionText.text = "";
        }

        /// <summary>
        /// Displays the request to select an army.
        /// </summary>
        public void SelectArmyRequest()
        {
            armyCompositionText.text = selectArmyRequestString;
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

        public void DisplayMaximumArmiesOnBoard()
        {
            armyCompositionText.text = exceedingArmyLimitString;
        }
    }
}