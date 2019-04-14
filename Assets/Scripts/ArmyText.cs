using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ArmyText : MonoBehaviour
    {
        public Text armyCompositionText;

        public void Init()
        {
            Clear();
            Enable();
        }

        public void ChangeText(string newText)
        {
            armyCompositionText.text = newText;
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
