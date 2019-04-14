using UnityEngine;

namespace Assets.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        private IntVector2 firstPlayerBlockPosition;
        private IntVector2 secondPlayerBlockPosition;
         
        private IntVector2 firstStartBlock = new IntVector2(1, 1);
        private IntVector2 secondStartBlock = new IntVector2(1, 1);
        public BlockBoardStorage BoardStorage { get; private set; }

        public void Initialize(BlockBoardStorage boardStorage)
        {
            BoardStorage = boardStorage;
            firstPlayerBlockPosition = firstStartBlock;
            secondPlayerBlockPosition = secondStartBlock;
        }

        public IBoardStorage GetCurrentBlock()
        {
            return BoardStorage.GetCurrentBlock();
        }
        
        public void SetActiveBlock(IntVector2 position)
        {
            BoardStorage.SetCurrentBlock(position);
        }

        public void SetPlayerBlockActive(TurnType playerType)
        {
            if (playerType == TurnType.FIRST)
            {
                BoardStorage.SetCurrentBlock(firstPlayerBlockPosition);
            }
            else if (playerType == TurnType.SECOND)
            {
                BoardStorage.SetCurrentBlock(secondPlayerBlockPosition);
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