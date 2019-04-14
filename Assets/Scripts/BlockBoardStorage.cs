using System.ComponentModel.Design;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockBoardStorage : IBoardStorage
    {
        private int width;
        private int height;

        private BoardStorage[,] blocks;
        private CheckeredButtonBoard board;

        private BoardStorage currentBlock;
        private IntVector2 currentBlockPosition;
        
        public BlockBoardStorage(int width, int height, CheckeredButtonBoard board)
        {
            this.width = width;
            this.height = height;
            this.board = board;
            
            blocks = new BoardStorage[width + 1, height + 1];
        }

        public void FillBlock(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems, IntVector2 blockPosition)
        {
            int blockWidth = items.GetLength(0) - 1;
            int blockHeight = items.GetLength(1) - 1;
            FillBlock(items, bonusItems, blockPosition, 1, blockWidth, 1,blockHeight);
        }

        private void FillBlock(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems,
            IntVector2 blockPosition, int fromX, int toX, int fromY, int toY)
        {
            int blockWidth = toX - fromX + 1;
            int blockHeight = toY - fromY + 1;
            blocks[blockPosition.x, blockPosition.y] = new BoardStorage(blockWidth, blockHeight, board);

            BoardStorage targetBlock = blocks[blockPosition.x, blockPosition.y];
            for (int col = fromX; col <= toX; col++)
            {
                for (int row = fromY; row <= toY; row++)
                {
                    int targetX = col - fromX + 1;
                    int targetY = row - fromY + 1;
                    targetBlock.SetItem(targetX, targetY, items[col, row]);
                    targetBlock.SetBonusItem(targetX, targetY, bonusItems[col, row]);
                }
            }
        }

        public void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems)
        {
            int blockWidth = (items.GetLength(0) - 1) / width;
            int blockHeight = (items.GetLength(1) - 1) / height;

            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    int fromX = (col - 1) * blockWidth + 1;
                    int toX = fromX + blockWidth - 1;
                    int fromY = (row - 1) * blockHeight + 1;
                    int toY = fromY + blockHeight - 1;
                    FillBlock(items, bonusItems, new IntVector2(col, row), fromX, toX, fromY, toY);
                }
            }
        }

        public void SetCurrentBlock(IntVector2 position)
        {
            currentBlock?.Deactivate();
            currentBlock = blocks[position.x, position.y];
            currentBlockPosition = position;
            currentBlock?.Activate();
        }

        public IntVector2 GetCurrentBlockPosition()
        {
            return currentBlockPosition;
        }
        
        public IBoardStorage GetCurrentBlock()
        {
            return currentBlock;
        }
        
        public void DisableBoardButtons()
        {
            currentBlock.DisableBoardButtons();
        }

        public void EnableBoardButtons()
        {
            currentBlock.EnableBoardButtons();
        }

        public void InvertBoard()
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    blocks[col, row].InvertBoard();
                }
            }
        }

        public int GetBoardWidth()
        {
            return currentBlock.GetBoardWidth();
        }

        public int GetBoardHeight()
        {
            return currentBlock.GetBoardHeight();
        }

        public void Reset()
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    blocks[col, row].Reset();
                }
            }
        }

        public BoardStorageItem GetItem(int positionX, int positionY)
        {
            return GetItem(new IntVector2(positionX, positionY));
        }
        
        public BoardStorageItem GetItem(IntVector2 position)
        {
            return currentBlock.GetItem(position);
        }

        public void SetItem(int col, int row, BoardStorageItem item)
        {
            SetItem(new IntVector2(col, row), item);
        }
        
        public void SetItem(IntVector2 position, BoardStorageItem item)
        {
            currentBlock.SetItem(position, item);
        }

        public BoardButton GetBoardButton(IntVector2 position)
        {
            return currentBlock.GetBoardButton(position);
        }

        public void DisableFrame(IntVector2 position)
        {
            currentBlock.DisableFrame(position);
        }

        public void EnableFrame(IntVector2 position)
        {
            currentBlock.EnableFrame(position);
        }

        public bool IsCastle(IntVector2 position)
        {
            return currentBlock.IsCastle(position);
        }

        public Castle GetCastle(IntVector2 position)
        {
            return currentBlock.GetCastle(position);
        }

        public BoardStorageItem GetBonusItem(IntVector2 position)
        {
            return currentBlock.GetBonusItem(position);
        }

        public bool ContainsPlayerArmies(PlayerType playerType)
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    if (blocks[col, row].ContainsPlayerArmies(playerType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void EnableArmies(PlayerType playerType)
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    blocks[col, row].EnableArmies(playerType);
                }
            }
        }
    }
}