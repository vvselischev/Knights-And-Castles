﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoardButton : MonoBehaviour
    {
        private int boardX;
        private int boardY;

        public GameIcon frame;

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
            DisableFrame();
            enabled = false;
        }
        
        public void DisableFrame()
        {
            frame.Disable();
        }

        public void EnableFrame()
        {
            frame.Enable();
        }
    }
}