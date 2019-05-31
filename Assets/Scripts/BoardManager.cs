using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
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

        public SingleBoardStorage GetCurrentBlock()
        {
            return boardStorage.GetCurrentBlock();
        }
        
        public void SetActiveBlock(IntVector2 position)
        {
            var currentBlockPosition = boardStorage.GetCurrentBlockPosition();
            boardStorage.SetCurrentBlock(position);
            if (currentBlockPosition != null && !currentBlockPosition.Equals(position))
            {
                Thread.Sleep(500);
            }
        }
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