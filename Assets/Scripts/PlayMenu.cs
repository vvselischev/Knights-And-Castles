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
        public Text scoreText;
        //public Text roundText;
        public StartButton startButton;
        public SplitButton splitButton;
        public CheckeredButtonBoard board;
        
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

        public void UpdateScoreText(RoundScore score)
        {
            scoreText.text = score.GetScore(PlayerType.FIRST) + " : " +
                score.GetScore(PlayerType.SECOND);
        }

        public void UpdateRoundText(int currentRound)
        {
            //roundText.text = "Round: " + currentRound;
        }

        public void DisableUI()
        {
            board.DisableBoard();
            startButton.Lock();
            splitButton.Lock();
        }

        public void EnableUI()
        {
            board.EnableBoard();
            startButton.Unlock();
            splitButton.Unlock();
        }
    }
}