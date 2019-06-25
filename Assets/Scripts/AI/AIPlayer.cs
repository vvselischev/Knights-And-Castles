namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of AI in the game.
    /// </summary>
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

        /// <summary>
        /// Activates AI so it starts to make moves.
        /// </summary>
        public void Activate()
        {
            MakeTurn();
        }

        /// <summary>
        /// In this method AI makes one turn, if AI is able to make one more turn method will be called again automatically
        /// </summary>
        private void MakeTurn()
        {
            controller.ArmyFinishedMove -= MakeTurn;
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

            OnTurnEnd(bestMove);
        }

        /// <summary>
        /// After AI finds move, this move should be done and
        /// state should be updated and MakeTurn method should be called again
        /// </summary>
        private void OnTurnEnd(MoveInformation bestMove)
        {
            //Add callback to this method when move will be performed
            controller.ArmyFinishedMove += MakeTurn;
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
