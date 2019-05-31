using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ArmyText : MonoBehaviour
    {
        [SerializeField] 
        private Text armyCompositionText;

        public void Init()
        {
            Clear();
            Enable();
        }

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

        public void Enable()
        {
            armyCompositionText.gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            armyCompositionText.gameObject.SetActive(false);
        }
    }
}
