using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        public BoardManager boardManager;
        private BlockBoardStorage boardStorage;
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

        private void MakeMove(Cell from, Cell to)
        {
            //Don't forget, that you must control the correctness of moves!
            //TODO: check the correctness.
            var fromBlockPosition = boardStorage.GetBlockPosition(from);
            var fromBlock = boardStorage.GetBlock(fromBlockPosition);
            var currentBlock = boardManager.GetCurrentBlock();
            
            if (fromBlock != currentBlock)
            {
                boardManager.SetActiveBlock(fromBlockPosition);

            }
            
            Debug.Log("AI move: fromBlock = " + fromBlockPosition);
            Debug.Log("Current block is " + boardStorage.GetCurrentBlockPosition());

            var positionFrom = boardStorage.GetPositionOnBoard(from);
            var positionTo = boardStorage.GetPositionOnBoard(to);
            inputListener.ProcessBoardClick(positionFrom.x, positionFrom.y);
            inputListener.ProcessBoardClick(positionTo.x, positionTo.y);
        }

        private void FinishTurn()
        {
            inputListener.ProcessFinishTurnClick();
        }
    }
}