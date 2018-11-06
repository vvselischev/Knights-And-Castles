using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoundVisualizer : MonoBehaviour
    {
        public MouseMover mover;

        public event VoidHandler OnFinishVisualize;
        public event VoidHandler MovedToNextPoint;
        public event VoidHandler ReachedCheese;
        public event VoidHandler FinishedLerp;

        public BoardStorage storage;
        public GameObject startMousePoint;
        public GameObject finishMousePoint;
        public int currentWayPointID;
        public UIManager uiManager;

        public Color color;

        void Awake()
        {
            mover.mouseGO.SetActive(false);
            uiManager.FinishedLerp += OnFinishLerp;
        }

        public void StartVisualize()
        {
            //works like a loop
            mover.ReachedTarget += TryGoNext;
            mover.ReachedTarget -= ReachedCheese;
            currentWayPointID = -1;
            mover.mouseGO.SetActive(true);
            mover.PrepareMovement(startMousePoint);
            TryGoNext();
        }

        //TODO: Добавить в классы Mouse и Cat метод PrintAsWinner (или Visitor сделать)
        public void PrintWinner(PlayerType type)
        {
            if (type == PlayerType.FIRST)
            {
                uiManager.PerformLerpString("FIRST VICTORY!", color);
            }
            else if (type == PlayerType.SECOND)
            {
                uiManager.PerformLerpString("SECOND VICTORY!", color);
            }
        }

        private void OnFinishLerp()
        {
            FinishedLerp();
        }

        public void FinishMovement()
        {
            mover.ReachedTarget -= TryGoNext;
        }

        private void TryGoNext()
        {
            /*if (currentWayPointID + 1 < storage.trajectory.path.Count)
            {
                mover.MoveTo(storage.trajectory.path[currentWayPointID + 1].mouseButton.gameObject);
                currentWayPointID++;
                MovedToNextPoint();
            }
            else
            {
                GoToFinish();
            }*/
        }

        private void GoToFinish()
        {
            mover.ReachedTarget -= TryGoNext;
            mover.ReachedTarget += ReachedCheese;
            mover.MoveTo(finishMousePoint);
        }

        public void FinishVisualize()
        {
            uiManager.DisableText();
            mover.gameObject.SetActive(false);

            Debug.Log("Finish vizualize");
        }
    }
}