using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        public BoardManager boardManager;
        private IBoardStorage boardStorage;
        public InputListener inputListener;
        public UserController controller;
        public PlayerType playerType;
        
        //Depth of tree for analyze. Should be odd!!!
        private const int DEPTH = 5;

        public void Initialize(UserController controller, PlayerType playerType)
        {
            this.playerType = playerType;
            this.controller = controller;
            boardStorage = boardManager.BoardStorage;
        }

        public void Activate()
        {
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
            
            // if it's better not to move for all armies
            if (bestMove == null) 
            {
                FinishTurn();
                return;
            }

            gameSimulation = new GameSimulation(boardStorage);
            gameSimulation.MakeMove(bestMove.From, bestMove.To, true);
            
            //Add callback to this method when move will be performed
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