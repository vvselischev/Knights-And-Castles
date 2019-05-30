using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BlockBoardStorage : IBoardStorage
    {
        private int width;
        private int height;

        private SingleBoardStorage[,] blocks;
        private CheckeredButtonBoard board;

        private SingleBoardStorage currentBlock;
        private IntVector2 currentBlockPosition;
        
        private Dictionary<PlayerType, List<Cell>> castles; // Where should be initialized?
        private Graph graph;  // Where should be initialized?

        public BlockBoardStorage(int width, int height, CheckeredButtonBoard board)
        {
            this.width = width;
            this.height = height;
            this.board = board;
            
            blocks = new SingleBoardStorage[width + 1, height + 1];
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
            blocks[blockPosition.x, blockPosition.y] = new SingleBoardStorage(blockWidth, blockHeight, board);

            SingleBoardStorage targetBlock = blocks[blockPosition.x, blockPosition.y];
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

        public SingleBoardStorage GetBlock(IntVector2 position)
        {
            return blocks[position.x, position.y];
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

        public void SetCurrentBlock(IntVector2 position)
        {
            currentBlock?.Deactivate();
            currentBlock = blocks[position.x, position.y];
            currentBlockPosition = position;
            currentBlock?.Activate();
            
            Debug.Log("Current block: " + position);
        }

        public IntVector2 GetCurrentBlockPosition()
        {
            return currentBlockPosition;
        }
        
        public SingleBoardStorage GetCurrentBlock()
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

         private void InvertPass(Pass pass, int oldBlockX, int oldBlockY)
        {
            var oldBlock = blocks[oldBlockX, oldBlockY];
            var oldToBlockPosition = pass.ToBlock;
            var toBlock = blocks[oldToBlockPosition.x, oldToBlockPosition.y];
            
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

        public List<Cell> FindPlayerArmies(PlayerType playerType)
        {
            var playerArmies = new List<Cell>();
            foreach (var block in blocks)
            {
                if (block != null)
                {
                    playerArmies.AddRange(block.FindPlayerArmies(playerType));
                }
            }

            return playerArmies;
        }

        public IBoardStorage CreateSimulationStorage()
        {
            var simulation = new BlockBoardStorage(width, height, board);
            simulation.currentBlockPosition = currentBlockPosition.CloneVector();
            for (int i = 1; i <= width; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    simulation.blocks[i, j] = blocks[i, j].CreateSimulationStorage() as SingleBoardStorage;
                }
            }

            simulation.currentBlock = simulation.blocks[currentBlockPosition.x, currentBlockPosition.y];
            return simulation;
        }

        public BoardStorageItem GetItem(Cell cell)
        {
            return (from SingleBoardStorage block in blocks where block != null && block.ContainsCell(cell)
                select block.GetItem(cell)).FirstOrDefault();
        }

        public void SetItem(Cell cell, BoardStorageItem item)
        {
            var blockWithCell =
                (from SingleBoardStorage block in blocks where block != null && block.ContainsCell(cell) select block)
                .FirstOrDefault();

            blockWithCell?.SetItem(cell, item);
        }

        public int GetDistanceToEnemyCastle(Cell cell, PlayerType playerType)
        {
            InitializeGraphAndListOfCastlesIfNot();
            var castleCell = castles[ChangePlayerType(playerType)][0];

            return graph.GetDistance(cell, castleCell);
        }

        private PlayerType ChangePlayerType(PlayerType playerType)
        {
            if (playerType == PlayerType.FIRST)
            {
                return PlayerType.SECOND;
            }

            return PlayerType.FIRST;
        }

        private void InitializeGraphAndListOfCastlesIfNot()
        {
            if (graph == null)
            {
                graph = new Graph(this);
            }

            if (castles == null)
            {
                FillCastles();
            }
        }

        public List<Cell> GetAdjacent(Cell cell)
        {
            var block = GetBlock(cell);
            var adjacentInSingleBoard = block.GetAdjacent(cell);
            var adjacent = new List<Cell>();

            foreach (var adjacentCell in adjacentInSingleBoard)
            {
                var item = block.GetItem(adjacentCell);
                var pass = item as Pass;
                if (pass != null)
                {
                    var toBlock = blocks[pass.ToBlock.x, pass.ToBlock.y];
                    adjacent.Add(toBlock.GetCellByPosition(pass.ToPosition));
                }
                else
                {
                    adjacent.Add(adjacentCell);
                }
            }

            return adjacent;
        }

        public List<Cell> FindActivePlayerArmies(PlayerType playerType)
        {
            var activePlayerArmies = new List<Cell>();
            foreach (var block in blocks) {
                if (block != null)
                {
                    activePlayerArmies.AddRange(block.FindActivePlayerArmies(playerType));
                }
            }

            return activePlayerArmies;
        }

        public IntVector2 GetPositionOnBoard(Cell cell) // Function is called in MakeMove and cell must be in current block
        {
            return currentBlock.GetPositionOnBoard(cell);
        }

        public SingleBoardStorage GetBlock(Cell cell)
        {
            var blockPosition = GetBlockPosition(cell);
            if (blockPosition == null)
            {
                return null;
            }

            return blocks[blockPosition.x, blockPosition.y];
        }

        public IntVector2 GetBlockPosition(Cell cell)
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    var block = blocks[col, row];
                    if (block.GetPositionOnBoard(cell) != null)
                    {
                        return new IntVector2(col, row);
                    }
                }
            }

            return null;
        }
        
        public int GetNumberOfCells()
        {
            int numberOfCells = 0;
            foreach (var block in blocks)
            {
                if (block != null)
                {
                    numberOfCells += block.GetNumberOfCells();
                }
            }

            return numberOfCells;
        }

        public List<Cell> GetListOfCells()
        {
            var listOfCells = new List<Cell>();
            foreach (var block in blocks)
            {
                if (block != null)
                {
                    listOfCells.AddRange(block.GetListOfCells());
                }
            }

            return listOfCells;
        }

        private void FillCastles()
        {
            castles = new Dictionary<PlayerType, List<Cell>>
            {
                {PlayerType.FIRST, new List<Cell>()}, {PlayerType.SECOND, new List<Cell>()}
            };
            foreach (var block in blocks)
            {
                if (block != null)
                {
                    var castleFirstPlayer = block.FindCastle(PlayerType.FIRST);
                    var castleSecondPlayer = block.FindCastle(PlayerType.SECOND);

                    if (castleFirstPlayer != null)
                    {
                        castles[PlayerType.FIRST].AddRange(castleFirstPlayer);
                    }

                    if (castleSecondPlayer != null)
                    {
                        castles[PlayerType.SECOND].AddRange(castleSecondPlayer);
                    }
                }
            }
        }

        public List<PassAsFromToCells> GetPassesAsFromToCells()
        {
            var passesAsFromToCells = new List<PassAsFromToCells>();

            foreach (var block in blocks)
            {
                if (block != null)
                {
                    var passes = block.GetPasses();
                    foreach (var pass in passes)
                    {
                        var toCell = blocks[pass.ToBlock.x, pass.ToBlock.y].GetCellByPosition(pass.ToPosition);
                        var fromCell = block.GetCellByPosition(pass.FromPosition);
                        passesAsFromToCells.Add(new PassAsFromToCells(fromCell, toCell));
                    }
                }
            }

            return passesAsFromToCells;
        }
    }
}