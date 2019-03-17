using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        public BoardStorage boardStorage;
        public InputListener inputListener;
        public UserController controller;
        public PlayerType playerType;
        private int turnNumber;
        
        //Depth of tree for analyze. Should be odd!!!
        private const int DEPTH = 5;

        public void Initialize(UserController controller, PlayerType playerType)
        {
            turnNumber = 0;
            this.playerType = playerType;
            this.controller = controller;
        }

        public void Activate()
        {
            turnNumber++;
            MakeTurn();
        }

        private void MakeTurn()
        {
            controller.FinishedMove -= MakeTurn;
            var gameSimulation = new GameSimulation(boardStorage);
            if (gameSimulation.GetNumberOfActiveArmies(playerType) == 0)
            {
                FinishTurn();
                return;
            }

            MoveInformation bestMove = gameSimulation.FindBestMove(playerType, DEPTH);

            if (bestMove == null) // if it's better not to move for all armies
            {
                FinishTurn();
                return;
            }

            gameSimulation = new GameSimulation(boardStorage);
            gameSimulation.MakeMove(bestMove.From, bestMove.To, true);
            controller.FinishedMove += MakeTurn;
            MakeMove(bestMove.From, bestMove.To);    
        }

        private void MakeMove(IntVector2 from, IntVector2 to)
        {
            //Don't forget, that you must control the correctness of moves!
            //TODO: check the correctness.
            inputListener.ProcessBoardClick(from.x, from.y);
            inputListener.ProcessBoardClick(to.x, to.y);
        }

        private void FinishTurn()
        {
            inputListener.ProcessFinishTurnClick();
        }
    }
}