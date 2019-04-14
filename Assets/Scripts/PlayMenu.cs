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
        public BoardManager boardManager;
        public ArmyText armyText;

        public Text playDebugText;
        
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
            boardManager.GetCurrentBlock().DisableBoardButtons();
            startButton.Lock();
            splitButton.Lock();
            armyText.Disable();
        }

        public void EnableUI()
        {
            boardManager.GetCurrentBlock().EnableBoardButtons();
            startButton.Unlock();
            splitButton.Unlock();
            armyText.Enable();
        }
    }
}