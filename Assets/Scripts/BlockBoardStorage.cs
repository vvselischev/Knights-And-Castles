using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

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

        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems)
        {
            //TODO: stupid solution, assuming that all blocks have equal size...
            int blockWidth = blocks[1, 1].GetBoardWidth();
            int blockHeight = blocks[1, 1].GetBoardHeight();

            items = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            bonusItems = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    int fromX = (col - 1) * blockWidth + 1;
                    int fromY = (row - 1) * blockHeight + 1;
                    for (int blockCol = 1; blockCol <= blockWidth; blockCol++)
                    {
                        for (int blockRow = 1; blockRow <= blockHeight; blockRow++)
                        {
                            items[fromX + blockCol - 1, fromY + blockRow - 1] =
                                blocks[col, row].GetItem(blockCol, blockRow);
                            bonusItems[fromX + blockCol - 1, fromY + blockRow - 1] =
                                blocks[col, row].GetBonusItem(blockCol, blockRow);
                        }
                    }
                }
            }
        }
        
        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems, Text logText)
        {
            //TODO: stupid solution, assuming that all blocks have equal size...
            int blockWidth = blocks[1, 1].GetBoardWidth();
            int blockHeight = blocks[1, 1].GetBoardHeight();

            items = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            bonusItems = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    int fromX = (col - 1) * blockWidth + 1;
                    int fromY = (row - 1) * blockHeight + 1;
                    for (int blockCol = 1; blockCol <= blockWidth; blockCol++)
                    {
                        for (int blockRow = 1; blockRow <= blockHeight; blockRow++)
                        {
                            logText.text = "fromX = " + fromX + ", " + "blockCol = " + blockCol + ", fromY = " +
                                            fromY + ", blockRow = " + blockRow + "\n";
                            items[fromX + blockCol - 1, fromY + blockRow - 1] =
                                blocks[col, row].GetItem(blockCol, blockRow);
                            bonusItems[fromX + blockCol - 1, fromY + blockRow - 1] =
                                blocks[col, row].GetBonusItem(blockCol, blockRow);
                        }
                    }
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
        
        public BoardStorage GetCurrentBlock()
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
            //Loops are completely separated, because after swapping blocks we cannot determine dimensions of
            //the target block for pass (actually, we can, but it is very painful)
            
            //Loop through blocks and invert passes
            for (int col = 1; col <= width / 2 + Math.Sign(width % 2); col++)
            {
                for (int row = 1; row <= height / 2 + Math.Sign(height % 2); row++)
                {
                    var invertedPosition = GetInvertedPosition(width, height, col, row);
                    int invertedCol = invertedPosition.x;
                    int invertedRow = invertedPosition.y;
                    
                    var firstBlock = blocks[col, row];
                    var secondBlock = blocks[invertedCol, invertedRow];
                    var firstPasses = firstBlock.GetPasses();
                    var secondPasses = secondBlock.GetPasses();

                    foreach (var pass in firstPasses)
                    {
                        InvertPass(pass, col, row);
                    }

                    foreach (var pass in secondPasses)
                    {
                        InvertPass(pass, invertedCol, invertedRow);
                    }
                }
            }
            
            //Invert all blocks
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    blocks[col, row].InvertBoard();
                }
            }
        }

        private void SwapBlocks(int firstCol, int firstRow, int secondCol, int secondRow)
        {
            var tmp = blocks[firstCol, firstRow];
            blocks[firstCol, firstRow] = blocks[secondCol, secondRow];
            blocks[secondCol, secondRow] = tmp;
        }

        private void InvertPass(Pass pass, int oldBlockX, int oldBlockY)
        {
            var oldBlock = blocks[oldBlockX, oldBlockY];
            var oldToBlockPosition = pass.toBlock;
            var toBlock = blocks[oldToBlockPosition.x, oldToBlockPosition.y];
            var newToBlockPosition = GetInvertedPosition(board.width, board.height,
                oldToBlockPosition.x, oldToBlockPosition.y);

            //pass.toBlock = newToBlockPosition;
            
            var fromPosition = pass.FromPosition;
            pass.FromPosition = GetInvertedPosition(oldBlock.GetBoardWidth(), oldBlock.GetBoardHeight(), fromPosition.x, fromPosition.y);
            
            var toPosition = pass.ToPosition;
            pass.ToPosition = GetInvertedPosition(toBlock.GetBoardWidth(), toBlock.GetBoardHeight(),toPosition.x, toPosition.y);
        }

        private IntVector2 GetInvertedPosition(int width, int height, int col, int row)
        {
            return new IntVector2(width - col + 1, height - row + 1);
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

        public BoardStorageItem GetBonusItem(int positionX, int positionY)
        {
            return currentBlock.GetBonusItem(positionX, positionY);
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

        public BoardStorage GetBlock(IntVector2 blockPosition)
        {
            return blocks[blockPosition.x, blockPosition.y];
        }
    }
}