
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        private IntVector2 firstPlayerBlockPosition;
        private IntVector2 secondPlayerBlockPosition;
        public BlockBoardStorage BoardStorage { get; private set; }

        public void Initialize(BlockBoardStorage boardStorage, IntVector2 firstStartBlock, IntVector2 secondStartBlock)
        {
            BoardStorage = boardStorage;
            firstPlayerBlockPosition = firstStartBlock;
            secondPlayerBlockPosition = secondStartBlock;
        }

        public SingleBoardStorage GetCurrentBlock()
        {
            return BoardStorage.GetCurrentBlock();
        }
        
        public void SetActiveBlock(IntVector2 position)
        {
            var currentBlockPosition = BoardStorage.GetCurrentBlockPosition();
            BoardStorage.SetCurrentBlock(position);
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
                var position =  BoardStorage.GetCurrentBlockPosition();
                if (position != null)
                {
                    firstPlayerBlockPosition = position;
                }
            }
            else if (playerType == TurnType.SECOND)
            {
                var position =  BoardStorage.GetCurrentBlockPosition();
                if (position != null)
                {
                    secondPlayerBlockPosition = position;
                }
            }
        }
    }
}