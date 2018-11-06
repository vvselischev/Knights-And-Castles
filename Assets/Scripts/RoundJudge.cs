using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RoundJudge : MonoBehaviour
    {
        public BoardStorage storage;
        private static RoundJudge instance;
        //public Mouse mouse;
        public RoundVisualizer visualizer;
        public PlayGameState playGameState;
        public PlayerType winner;
        public VoidHandler OnFinishJudge;

        private bool needContinue;

        public void StartJudge()
        {
            visualizer.StartVisualize();
        }

        public void FinishJudge()
        {
            visualizer.FinishMovement();
            playGameState.roundScore.IncreaseScore(winner);
            visualizer.PrintWinner(winner);
            needContinue = false;
        }

        public static RoundJudge GetInstance()
        {
            return instance;
        }

        public void Awake()
        {
            instance = this;
            visualizer.MovedToNextPoint += CheckWayPoint;
            visualizer.ReachedCheese += MouseWin;
            visualizer.FinishedLerp += FinishVisualize;
        }

        private void FinishVisualize()
        {
            visualizer.FinishVisualize();
            OnFinishJudge();
        }

        public void CheckWayPoint()
        {
            int id = visualizer.currentWayPointID;
           /* WayPoint currentWayPoint = storage.trajectory.path[id];
            int boardX = currentWayPoint.mouseButton.boardButton.boardX;
            int boardY = currentWayPoint.mouseButton.boardButton.boardY;
            List<BoardStorageItem> bonuses = new List<BoardStorageItem>();
            bonuses = storage.boardTable[boardX, boardY];


            foreach (BoardStorageItem item in bonuses)
            {
                needContinue = true;
                item.bonus.Execute();

                //Чтобы не перебирать, если убились
                if (!needContinue)
                {
                    break;
                }
            }*/
        }

        public void MouseWin()
        {
            winner = PlayerType.FIRST;
            FinishJudge();
        }

        public void TryToKill()
        {
           /* if (!mouse.IsArmoured()) //can kill
            {
                mouse.Kill();
                winner = PlayerType.CAT;
                FinishJudge();
            }
            else //can't kill
            {
                //TODO: some effect
            }*/
        }
    }
}