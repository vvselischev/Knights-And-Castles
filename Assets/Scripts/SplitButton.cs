using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    //TODO: make it and start button children of common class
    public class SplitButton : MonoBehaviour
    {
        public GameIcon icon;
        public Color defaultColor = Color.white;
        public Color lockColor = Color.red;

        public void OnClick()
        {
            if (enabled)
            {
                //
            }
        }
        
        public void Lock()
        {
            enabled = false;
            icon.ChangeColor(lockColor);
        }

        public void Unlock()
        {
            enabled = true;
            icon.ChangeColor(defaultColor);
        }
    }
}