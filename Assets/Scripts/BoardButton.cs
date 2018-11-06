using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoardButton : MonoBehaviour
    {
        [NonSerialized]
        public int boardX;
        [NonSerialized]
        public int boardY;
        [NonSerialized]
        public Button button;

        public GameIcon icon;

        public ButtonListener bonusManager;

        public void Initialize(int x, int y)
        {
            boardX = x;
            boardY = y;
            button = GetComponent<Button>();
        }

        public void OnClick()
        {
            Debug.Log("Board button clicked");
            bonusManager.OnBoardClick(this);
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}