using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ArmyTextManager : MonoBehaviour
    {
        public Text armyCompositionText;

        public void Init()
        {
            armyCompositionText.text = "";
        }

        public void ChangeText(string newText)
        {
            armyCompositionText.text = newText;
        }
    }
}
