using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of multiblocked board
    /// </summary>
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

        /// <summary>
        /// Fills block at given position with given arrays of army and bonus items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bonusItems"></param>
        /// <param name="blockPosition"></param>
        public void FillBlock(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems, IntVector2 blockPosition)
        {
            var blockWidth = items.GetLength(0) - 1;
            var blockHeight = items.GetLength(1) - 1;
            FillBlock(items, bonusItems, blockPosition, 1, blockWidth, 1,blockHeight);
        }

        private void FillBlock(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems,
            IntVector2 blockPosition, int fromX, int toX, int fromY, int toY)
        {
            var blockWidth = toX - fromX + 1;
            var blockHeight = toY - fromY + 1;
            blocks[blockPosition.x, blockPosition.y] = new SingleBoardStorage(blockWidth, blockHeight, board);

            var targetBlock = blocks[blockPosition.x, blockPosition.y];
            for (var col = fromX; col <= toX; col++)
            {
                for (var row = fromY; row <= toY; row++)
                {
                    var targetX = col - fromX + 1;
                    var targetY = row - fromY + 1;
                    targetBlock.SetItem(targetX, targetY, items[col, row]);
                    targetBlock.SetBonusItem(targetX, targetY, bonusItems[col, row]);
                }
            }
        }

        /// <summary>
        /// Returns block by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SingleBoardStorage GetBlock(IntVector2 position)
        {
            return blocks[position.x, position.y];
        }

        /// <summary>
        /// Fills whole board with items from arrays given from arguments
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bonusItems"></param>
        public void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems)
        {
            var blockWidth = (items.GetLength(0) - 1) / width;
            var blockHeight = (items.GetLength(1) - 1) / height;

            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    var fromX = (col - 1) * blockWidth + 1;
                    var toX = fromX + blockWidth - 1;
                    var fromY = (row - 1) * blockHeight + 1;
                    var toY = fromY + blockHeight - 1;
                    FillBlock(items, bonusItems, new IntVector2(col, row), fromX, toX, fromY, toY);
                }
            }
        }

        /// <summary>
        /// Gets all army and bonus items from board and puts them to arrays given as arguments
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bonusItems"></param>
        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems)
        {
            //TODO: stupid solution, assuming that all blocks have equal size...
            var blockWidth = blocks[1, 1].GetBoardWidth();
            var blockHeight = blocks[1, 1].GetBoardHeight();

            items = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            bonusItems = new BoardStorageItem[width * blockWidth + 1, height * blockHeight + 1];
            
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    var fromX = (col - 1) * blockWidth + 1;
                    var fromY = (row - 1) * blockHeight + 1;
                    for (var blockCol = 1; blockCol <= blockWidth; blockCol++)
                    {
                        for (var blockRow = 1; blockRow <= blockHeight; blockRow++)
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

        /// <summary>
        /// Changes current block to block at given position
        /// </summary>
        /// <param name="position"></param>
        public void SetCurrentBlock(IntVector2 position)
        {
            currentBlock?.Deactivate();
            currentBlock = blocks[position.x, position.y];
            currentBlockPosition = position;
            currentBlock?.Activate();
        }

        /// <summary>
        /// Returns position of current block
        /// </summary>
        /// <returns></returns>
        public IntVector2 GetCurrentBlockPosition()
        {
            return currentBlockPosition;
        }
        
        /// <summary>
        /// Returns current block
        /// </summary>
        /// <returns></returns>
        public SingleBoardStorage GetCurrentBlock()
        {
            return currentBlock;
        }
        
        /// <summary>
        /// Disables all board butons in current block
        /// </summary>
        public void DisableBoardButtons()
        {
            currentBlock.DisableBoardButtons();
        }

        /// <summary>
        /// Enables all board buttons in current block
        /// </summary>
        public void EnableBoardButtons()
        {
            currentBlock.EnableBoardButtons();
        }

        /// <summary>
        /// Rotates board on 180 angles
        /// </summary>
        public void InvertBoard()
        {
            //Loops are completely separated, because after swapping blocks we cannot determine dimensions of
            //the target block for pass (actually, we can, but it is very painful)
            
            //Loop through blocks and invert passes
            for (var col = 1; col <= width / 2 + Math.Sign(width % 2); col++)
            {
                for (var row = 1; row <= height / 2 + Math.Sign(height % 2); row++)
                {
                    var invertedPosition = GetInvertedPosition(width, height, col, row);
                    var invertedCol = invertedPosition.x;
                    var invertedRow = invertedPosition.y;
                    
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
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
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

        /// <summary>
        /// Returns number of blocks in width 
        /// </summary>
        /// <returns></returns>
        public int GetBoardWidth()
        {
            return currentBlock.GetBoardWidth();
        }

        /// <summary>
        /// Returns number of blocks in height
        /// </summary>
        /// <returns></returns>
        public int GetBoardHeight()
        {
            return currentBlock.GetBoardHeight();
        }

        public void Reset()
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    blocks[col, row].Reset();
                }
            }
        }

        /// <summary>
        /// Returns item by given position in current block
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(int positionX, int positionY)
        {
            return GetItem(new IntVector2(positionX, positionY));
        }
        
        /// <summary>
        /// Returns item by given position in current block
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(IntVector2 position)
        {
            return currentBlock.GetItem(position);
        }

        /// <summary>
        /// Returns bonus item by given position in current block
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        public BoardStorageItem GetBonusItem(int positionX, int positionY)
        {
            return currentBlock.GetBonusItem(positionX, positionY);
        }

        /// <summary>
        /// Sets item by given position in current block
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="item"></param>
        public void SetItem(int col, int row, BoardStorageItem item)
        {
            SetItem(new IntVector2(col, row), item);
        }
        
        /// <summary>
        /// Sets item by given position in current block
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        public void SetItem(IntVector2 position, BoardStorageItem item)
        {
            currentBlock.SetItem(position, item);
        }

        /// <summary>
        /// Returns board button by given position in current block
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardButton GetBoardButton(IntVector2 position)
        {
            return currentBlock.GetBoardButton(position);
        }

        /// <summary>
        /// Disables from by given position in current block
        /// </summary>
        /// <param name="position"></param>
        public void DisableFrame(IntVector2 position)
        {
            currentBlock.DisableFrame(position);
        }

        /// <summary>
        /// Enables frame by given position in current block
        /// </summary>
        /// <param name="position"></param>
        public void EnableFrame(IntVector2 position)
        {
            currentBlock.EnableFrame(position);
        }

        /// <summary>
        /// Checks that in current block by given position castle is located
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsCastle(IntVector2 position)
        {
            return currentBlock.IsCastle(position);
        }

        /// <summary>
        /// Returns castle by given position from current block
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Castle GetCastle(IntVector2 position)
        {
            return currentBlock.GetCastle(position);
        }

        /// <summary>
        /// Returns bonus item by given position from current block 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardStorageItem GetBonusItem(IntVector2 position)
        {
            return currentBlock.GetBonusItem(position);
        }

        /// <summary>
        /// Checks that boards contains at least one cell with army of given player type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool ContainsPlayerArmies(PlayerType playerType)
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    if (blocks[col, row].ContainsPlayerArmies(playerType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets all armies of given user active
        /// </summary>
        /// <param name="playerType"></param>
        public void EnableArmies(PlayerType playerType)
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    blocks[col, row].EnableArmies(playerType);
                }
            }
        }

        /// <summary>
        /// Finds cells, which contains given player armies
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates copy of board for AI
        /// </summary>
        /// <returns></returns>
        public IBoardStorage CreateSimulationStorage()
        {
            var simulation = new BlockBoardStorage(width, height, board)
            {
                currentBlockPosition = currentBlockPosition.CloneVector()
            };
            for (var i = 1; i <= width; i++)
            {
                for (var j = 1; j <= height; j++)
                {
                    simulation.blocks[i, j] = blocks[i, j].CreateSimulationStorage() as SingleBoardStorage;
                }
            }

            simulation.currentBlock = simulation.blocks[currentBlockPosition.x, currentBlockPosition.y];
            return simulation;
        }

        /// <summary>
        /// Returns army item by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(Cell cell)
        {
            return (from SingleBoardStorage block in blocks 
                where block != null && block.ContainsCell(cell)
                select block.GetItem(cell))
                .FirstOrDefault();
        }

        /// <summary>
        /// Sets army item by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="item"></param>
        public void SetItem(Cell cell, BoardStorageItem item)
        {
            var blockWithCell =
                (from SingleBoardStorage block in blocks
                    where block != null && block.ContainsCell(cell) 
                    select block)
                .FirstOrDefault();

            blockWithCell?.SetItem(cell, item);
        }

        /// <summary>
        /// Returns distance from cell to cell with enemy castle
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public int GetDistanceToEnemyCastle(Cell cell, PlayerType playerType)
        {
            InitializeGraphAndListOfCastlesIfNot();
            var castleCell = castles[GetOpponentPlayerType(playerType)][0];

            return graph.GetDistance(cell, castleCell);
        }

        public static PlayerType GetOpponentPlayerType(PlayerType playerType)
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

        /// <summary>
        /// Returns cells which are one step away from given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public IEnumerable<Cell> GetAdjacent(Cell cell)
        {
            var block = GetBlock(cell);
            var adjacentInSingleBoard = block.GetAdjacent(cell);
            var adjacent = new List<Cell>();

            foreach (var adjacentCell in adjacentInSingleBoard)
            {
                var item = block.GetItem(adjacentCell);
                if (item is Pass pass)
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

        /// <summary>
        /// Returns cells with active armies of given players
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
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

        // Function is called in MakeMove and cell must be in current block
        /// <summary>
        /// Returns position of cell in current block
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public IntVector2 GetPositionOnBoard(Cell cell)
        {
            return currentBlock.GetPositionOnBoard(cell);
        }

        /// <summary>
        /// Get block containing given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public SingleBoardStorage GetBlock(Cell cell)
        {
            var blockPosition = GetBlockPosition(cell);
            if (blockPosition == null)
            {
                return null;
            }

            return blocks[blockPosition.x, blockPosition.y];
        }

        /// <summary>
        /// Get block position, which contains given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public IntVector2 GetBlockPosition(Cell cell)
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
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
            return (from SingleBoardStorage block in blocks 
                    where block != null 
                    select block.GetNumberOfCells())
                    .Sum();
        }

        public IEnumerable<Cell> GetListOfCells()
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
                if (block == null)
                {
                    continue;
                }
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

        /// <summary>
        /// Returns all board passes in format from and to cells
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PassAsFromToCells> GetPassesAsFromToCells()
        {
            var passesAsFromToCells = new List<PassAsFromToCells>();

            foreach (var block in blocks)
            {
                if (block == null)
                {
                    continue;
                }
                
                var passes = block.GetPasses();
                foreach (var pass in passes)
                {
                    var toCell = blocks[pass.ToBlock.x, pass.ToBlock.y].GetCellByPosition(pass.ToPosition);
                    var fromCell = block.GetCellByPosition(pass.FromPosition);
                    passesAsFromToCells.Add(new PassAsFromToCells(fromCell, toCell));
                }
            }

            return passesAsFromToCells;
        }
        
        public void FillBlockForTesting(IntVector2 blockPosition, BoardStorageItem[,] items, BoardStorageItem[,] bonusItems) 
        { 
            blocks[blockPosition.x, blockPosition.y] = new SingleBoardStorage(2, 2, null); 
            blocks[blockPosition.x, blockPosition.y].Fill(items, bonusItems); 
        }
    }
}