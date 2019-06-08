namespace Assets.Scripts
{
    /// <summary>
    /// Class for switching between different block on board.
    /// Can memorise the player block and automatically switch to it if necessary.
    /// The current block -- is the block displayed on the screen.
    /// </summary>
    public class BoardManager
    {
        private IntVector2 firstPlayerBlockPosition;
        private IntVector2 secondPlayerBlockPosition;
        private BlockBoardStorage boardStorage;

        public BoardManager(BlockBoardStorage boardStorage, IntVector2 firstStartBlock, IntVector2 secondStartBlock)
        {
            this.boardStorage = boardStorage;
            firstPlayerBlockPosition = firstStartBlock;
            secondPlayerBlockPosition = secondStartBlock;
        }

        /// <summary>
        /// Returns the current block.
        /// </summary>
        /// <returns></returns>
        public SingleBoardStorage GetCurrentBlock()
        {
            return boardStorage.GetCurrentBlock();
        }
        
        /// <summary>
        /// Sets the block by the given position active.
        /// </summary>
        public void SetActiveBlock(IntVector2 position)
        {
            boardStorage.SetCurrentBlock(position);
        }
        
        /// <summary>
        /// Sets active previously memorised player block.
        /// </summary>
        public void SetPlayerBlockActive(TurnType playerType)
        {
            if (playerType == TurnType.FIRST)
            {
                SetActiveBlock(firstPlayerBlockPosition);
            }
            else if (playerType == TurnType.SECOND)
            {
                SetActiveBlock(secondPlayerBlockPosition);
            }
        }

        /// <summary>
        /// Memorises the current block for the given player.
        /// </summary>
        public void SavePlayerBlock(TurnType playerType)
        {
            if (playerType == TurnType.FIRST)
            {
                var position =  boardStorage.GetCurrentBlockPosition();
                if (position != null)
                {
                    firstPlayerBlockPosition = position;
                }
            }
            else if (playerType == TurnType.SECOND)
            {
                var position =  boardStorage.GetCurrentBlockPosition();
                if (position != null)
                {
                    secondPlayerBlockPosition = position;
                }
            }
        }
    }
}