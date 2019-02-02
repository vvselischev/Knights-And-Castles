using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoardButton : MonoBehaviour
    {
        //TODO: make private
        [NonSerialized]
        public int boardX;
        [NonSerialized]
        public int boardY;

        public GameIcon icon;

        public InputListener inputListener;

        public void Initialize(int x, int y)
        {
            boardX = x;
            boardY = y;
        }

        public void OnClick()
        {
            if (enabled)
            {
                Debug.Log("Board button clicked");
                inputListener.ProcessBoardClick(boardX, boardY);
            }
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