using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class StartButton : MonoBehaviour
    {
        public GameIcon icon;
        public Color defaultColor = Color.white;
        public Color lockColor = Color.red;

        public InputListener inputListener;

        public void OnClick()
        {
            if (enabled)
            {
                inputListener.ProcessFinishTurnClick();
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
            icon.Reset();
        }
    }
}