using UnityEngine;

namespace Assets.Scripts
{
    public class AIPlayer
    {
        private BoardManager boardManager;
        private BlockBoardStorage boardStorage;
        private InputListener inputListener;

        private UserController controller;
        private PlayerType playerType;

        public AIPlayer(UserController controller, PlayerType playerType, BlockBoardStorage boardStorage, 
            BoardManager boardManager, InputListener inputListener)
        {
            this.playerType = playerType;
            this.controller = controller;
            this.boardManager = boardManager;
            this.inputListener = inputListener;
            this.boardStorage = boardStorage;
        }

        public void Activate()
        {
            MakeTurn();
        }

        private void MakeTurn()
        {
            controller.FinishedMove -= MakeTurn;
            
            var gameSimulation = new GameSimulation(boardStorage);

            var myArmiesNumber = gameSimulation.GetNumberOfActiveArmies(playerType);
            
            if (myArmiesNumber == 0)
            {
                FinishTurn();
                return;
            }

            var bestMove = gameSimulation.FindBestMove(playerType);
            
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