using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayMenu : MonoBehaviour, IMenu
    {
        public Canvas canvas;
        public StartButton startButton;
        public SplitButton splitButton;
        public CheckeredButtonBoard board;
        public ArmyText armyText;
        
        public void Activate()
        {
            gameObject.SetActive(true);
            canvas.enabled = true;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            canvas.enabled = false;
        }

        public void DisableUI()
        {
            board.DisableBoard();
            startButton.Lock();
            splitButton.Lock();
            armyText.Disable();
        }

        public void EnableUI()
        {
            board.EnableBoard();
            startButton.Unlock();
            splitButton.Unlock();
            armyText.Enable();
        }
    }
}