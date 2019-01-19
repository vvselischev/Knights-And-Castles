using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        public BoardStorage boardStorage;
        public InputListener inputListener;

        //Move -- move of one of your armies.
        private static int moveNumber;   
        private static int turnNumber;   

        public void Initialize(UserController controller)
        {
            turnNumber = 0;
            controller.FinishedMove += OnFinishMove;
        }
        public void Activate()
        {
            moveNumber = 0;
            turnNumber++;
            MakeMove();
        }

        private void MakeMove()
        {
            //Analyze board storage...
            //Remember that the board is not inverted, so you start from (6, 6).

            moveNumber++;
            if ((turnNumber == 1) && (moveNumber == 1))
            {
                MakeMove(6, 6, 6, 5);
                return;
                //MakeMove will be called again automatically when icon reaches the target.     
                //But write return after every call of MakeMove(int, int, int, int) to ensure that it is called only once!
            } 
            else if ((turnNumber == 2) && (moveNumber == 1))
            {
                MakeMove(6, 5, 5, 5);
                return;
            }

            //Don't forget to finish turn! 
            //Just leave it here and if there is a return statement after each MakeMove, you don't need to care.
            FinishTurn();
        }


        private void MakeMove(int fromX, int fromY, int toX, int toY)
        {
            //Don't forget, that you must control the correctness of moves!
            //TODO: check the correctness.
            inputListener.ProcessBoardClick(boardStorage.GetBoardButton(fromX, fromY));
            inputListener.ProcessBoardClick(boardStorage.GetBoardButton(toX, toY));
        }

        private void OnFinishMove()
        {
            MakeMove();
        }

        private void FinishTurn()
        {
            inputListener.ProcessFinishTurnClick();
        }
    }
}