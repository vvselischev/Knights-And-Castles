using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class SplitButton : MonoBehaviour
    {
        public GameIcon icon;
        public Color defaultColor = Color.green;
        public Color lockColor = Color.red;

        public void Lock()
        {
            icon.ChangeColor(lockColor);
        }

        public void Unlock()
        {
            icon.ChangeColor(defaultColor);
        }
    }
}