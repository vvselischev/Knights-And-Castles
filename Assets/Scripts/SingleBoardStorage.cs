﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.PlayerLoop;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of primitive board block
    /// </summary>
    public class SingleBoardStorage : IBoardStorage
    {
        /// <summary>
        /// Stores items with armies
        /// </summary>
        private BoardStorageItem[,] boardTable;
        
        /// <summary>
        /// Stores bonus items
        /// </summary>
        private BoardStorageItem[,] bonusTable;
        
        /// <summary>
        /// Stores board cells
        /// </summary>
        private Cell[,] cells;
        
        /// <summary>
        /// Maps cells to their positions a the board
        /// </summary>
        private Dictionary<Cell, IntVector2> indexByCell = new Dictionary<Cell, IntVector2>();
        
        /// <summary>
        /// Stores board width
        /// </summary>
        private int width;

        /// <summary>
        /// Stores board height
        /// </summary>
        private int height;
        
        //TODO: remove this dependency?
        private CheckeredButtonBoard board;
        
        public SingleBoardStorage(int width, int height, CheckeredButtonBoard board)
        {
            this.board = board;
            Initialize(width, height);  
        }

        /// <summary>
        /// Creates tables with bonus items and army items
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Initialize(int width, int height)
        {
            this.width = width;
            this.height = height;
            boardTable = new BoardStorageItem[width + 1, height + 1];
            bonusTable = new BoardStorageItem[width + 1, height + 1];
            cells = new Cell[width + 1, height + 1];
            CreateCells();
        }

        /// <summary>
        /// Creates cells and maps them to their positions
        /// </summary>
        private void CreateCells()
        {
            for (var i = 1; i <= width; i++)
            {
                for (var j = 1; j <= height; j++)
                {
                    var cell = new Cell();
                    indexByCell.Add(cell, new IntVector2(i, j));
                    cells[i, j] = cell;
                }
            }
        }

        /// <summary>
        /// Activate board
        /// </summary>
        public void Activate()
        {
            SetAllItemsActive(true);
        }

        /// <summary>
        /// Deactivates board
        /// </summary>
        public void Deactivate()
        {
            SetAllItemsActive(false);
        }

        /// <summary>
        /// Sets all items in tables active
        /// </summary>
        /// <param name="active"></param>
        private void SetAllItemsActive(bool active)
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    boardTable[col, row]?.StoredObject.SetActive(active);
                    bonusTable[col, row]?.StoredObject.SetActive(active);
                }
            }
        }

        
        /// <summary>
        /// Returns board height
        /// </summary>
        /// <returns></returns>
        public int GetBoardHeight()
        {
            return height;
        }

        /// <summary>
        /// Fills given arrays from storing table
        /// </summary>
        /// <param name="items"> Fills from table with army items </param>
        /// <param name="bonusItems"> Fills from table with bonus items </param>
        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems)
        {
            items = new BoardStorageItem[width + 1, height + 1];
            bonusItems = new BoardStorageItem[width + 1, height + 1];
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    items[col, row] = boardTable[col, row];
                    bonusItems[col, row] = bonusItems[col, row];
                }
            }
        }

        /// <summary>
        /// Returns board width
        /// </summary>
        /// <returns></returns>
        public int GetBoardWidth()
        {
            return width;
        }
        
        /// <summary>
        /// Enables all board buttons
        /// </summary>
        public void EnableBoardButtons()
        {
            board.EnableBoard();
        }

        
        /// <summary>
        /// Disables all board buttons
        /// </summary>
        public void DisableBoardButtons()
        {
            board.DisableBoard();
        }
        
        /// <summary>
        /// Returns item from table with army items by given position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(int positionX, int positionY)
        {
            return boardTable[positionX, positionY];
        }

        /// <summary>
        /// Sets item to table with army items by given position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="item"></param>
        public void SetItem(int positionX, int positionY, BoardStorageItem item)
        {
            SetItem(positionX, positionY, item, boardTable);
        }

        /// <summary>
        /// Returns bonus item by given position 
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        public BoardStorageItem GetBonusItem(int positionX, int positionY)
        {
            return bonusTable[positionX, positionY];
        }

        /// <summary>
        /// Sets item to table with bonus items by given position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="item"></param>
        public void SetBonusItem(int positionX, int positionY, BoardStorageItem item)
        {
            SetItem(positionX, positionY, item, bonusTable);
        }
        
        private void SetItem(int positionX, int positionY, BoardStorageItem item, BoardStorageItem[,] table)
        {
            table[positionX, positionY] = item;
            var targetButton = GetBoardButton(positionX, positionY);
            if (item != null && table[positionX, positionY].StoredObject != null)
            {
                table[positionX, positionY].StoredObject.transform.position = 
                    targetButton.transform.position;
            }
        }
        
        /// <summary>
        /// Returns item from table with army items by given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(IntVector2 position)
        {
            return GetItem(position.x, position.y);
        }

        /// <summary>
        /// Sets item to table with army items by given position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="item"></param>
        public void SetItem(IntVector2 position, BoardStorageItem item)
        {
            SetItem(position.x, position.y, item);
        }

        /// <summary>
        /// Returns board button by given position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        public BoardButton GetBoardButton(int positionX, int positionY)
        {
            return GetBoardButton(new IntVector2(positionX, positionY));
        }
        
        /// <summary>
        /// Returns board button by given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardButton GetBoardButton(IntVector2 position)
        {
            return board.GetBoardButton(position);
        }

        /// <summary>
        /// Adds castle to table with bonus items by given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="castle"></param>
        public void AddCastle(IntVector2 position, Castle castle)
        {
            bonusTable[position.x, position.y] = castle;
        }

        /// <summary>
        /// Returns castle by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Castle GetCastle(IntVector2 position)
        {
            return bonusTable[position.x, position.y] as Castle;
        }

        /// <summary>
        /// Returns item from bonus table by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BoardStorageItem GetBonusItem(IntVector2 position)
        {
            return bonusTable[position.x, position.y];
        }

        /// <summary>
        /// Checks that board contains at least one army with given player type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool ContainsPlayerArmies(PlayerType playerType)
        {
            return FindPlayerArmies(playerType).Count > 0;
        }

        /// <summary>
        /// Sets all user armies active
        /// </summary>
        /// <param name="playerType"></param>
        public void EnableArmies(PlayerType playerType)
        {
            for (var i = 1; i <= GetBoardHeight(); i++)
            {
                for (var j = 1; j <= GetBoardWidth(); j++)
                {
                    if (GetItem(j, i) is ArmyStorageItem item)
                    {
                        var userArmy = item.Army as UserArmy;
                        if (item.Army.PlayerType == playerType)
                        {
                            userArmy?.SetActive();
                        }
                    }
                }
            }
        }

        
        /// <summary>
        /// Returns list of cells with armies of given player
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<Cell> FindPlayerArmies(PlayerType playerType)
        {
            var cellsWithArmies = new List<Cell>();
            foreach (var dictionaryEntry in indexByCell)
            {
                var boardItem = GetItem(dictionaryEntry.Value);
                if (boardItem is ArmyStorageItem item)
                {
                    if (item.Army.PlayerType == playerType)
                    {
                        cellsWithArmies.Add(dictionaryEntry.Key);
                    }
                }
            }
    
            return cellsWithArmies;
        }

        /// <summary>
        /// Creates copy of board for AI
        /// </summary>
        /// <returns></returns>
        public IBoardStorage CreateSimulationStorage()
        {
            var simulationStorage = new SingleBoardStorage(GetBoardWidth(), GetBoardHeight(), board)
            {
                cells = cells, indexByCell = indexByCell, bonusTable = bonusTable
            };

            for (var col = 1; col <= GetBoardWidth(); col++)
            {
                for (var row = 1; row <= GetBoardHeight(); row++)
                {
                    var item = boardTable[col, row];
                    if (item is ArmyStorageItem storageItem)
                    {
                        simulationStorage.boardTable[col, row] = storageItem.CloneWithoutIcon();
                    }
                    else
                    {
                        simulationStorage.boardTable[col, row] = null;
                    }
                }
            }

            return simulationStorage;
        }

        /// <summary>
        /// Checks that board contains given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool ContainsCell(Cell cell)
        {
            return indexByCell.ContainsKey(cell);
        }

        
        /// <summary>
        /// Returns item from table with army items by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public BoardStorageItem GetItem(Cell cell)
        {
            if (ContainsCell(cell))
            {
                return GetItem(indexByCell[cell]);
            }

            return null;
        }

        /// <summary>
        /// Sets item to table with army items by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="item"></param>
        public void SetItem(Cell cell, BoardStorageItem item)
        {
            SetItem(indexByCell[cell], item);
        }

        
        /// <summary>
        /// Returns distance from given cell to cell with enemy castle
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public int GetDistanceToEnemyCastle(Cell cell, PlayerType playerType)
        {
            var position = indexByCell[cell];
            var castlePosition = GetCastlePosition(playerType);

            return Math.Abs(position.x - castlePosition.x) + Math.Abs(position.y - castlePosition.y);
        }

        private IntVector2 GetCastlePosition(PlayerType playerType)
        {
            if (playerType == PlayerType.FIRST)
            {
                return new IntVector2(1, 1);
            }

            if (playerType == PlayerType.SECOND)
            {
                return new IntVector2(GetBoardWidth(), GetBoardHeight());
            }
            
            throw new ArgumentException("PlayerType is not appropriate");
        }

        /// <summary>
        /// Finds player castles
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<Cell> FindCastle(PlayerType playerType)
        {
            var castlePosition = GetCastlePosition(playerType);

            if (bonusTable[castlePosition.x, castlePosition.y] is Castle)
            {
                return new List<Cell> {cells[castlePosition.x, castlePosition.y]};
            }

            return null;
        }


        /// <summary>
        /// Returns cells which are one step away from given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public List<Cell> GetAdjacent(Cell cell)
        {
            var position = indexByCell[cell];
            var possibleNeighbours = new List<IntVector2>
                {position.IncrementX(), position.DecrementX(), position.IncrementY(), position.DecrementY()};
            var neighbours = new List<Cell>();
            
            foreach (var possibleNeighbour in possibleNeighbours) 
            {
                if (IsValidPosition(possibleNeighbour))
                {
                    neighbours.Add(cells[possibleNeighbour.x, possibleNeighbour.y]);
                }
            }

            return neighbours;
        }

        private bool IsValidPosition(IntVector2 position)
        {
            return position.x > 0 && position.x <= width && position.y > 0 && position.y <= height;
        }

        /// <summary>
        /// Finds cells with active player armies
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<Cell> FindActivePlayerArmies(PlayerType playerType)
        {
            var cellsWithArmies = FindPlayerArmies(playerType);
            var cellsWithActiveArmies = new List<Cell>();

            foreach (var cellWithArmy in cellsWithArmies)
            {
                if (IsActiveArmyInCell(cellWithArmy))
                {
                    cellsWithActiveArmies.Add(cellWithArmy);
                }
            }

            return cellsWithActiveArmies;
        }

        /// <summary>
        /// Returns position on a board by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public IntVector2 GetPositionOnBoard(Cell cell)
        {
            if (indexByCell.ContainsKey(cell))
            {
                return indexByCell[cell];
            }

            return null;
        }

        /// <summary>
        /// Returns board block which contains given cell 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public SingleBoardStorage GetBlock(Cell cell)
        {
            return this;
        }

        private bool IsActiveArmyInCell(Cell cell)
        {
            var item = GetItem(indexByCell[cell]) as ArmyStorageItem;
            return item?.Army is UserArmy army && army.IsActive();
        }

        /// <summary>
        /// Checks that in table with bonus items by given position is castle
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsCastle(IntVector2 position)
        {
            return bonusTable[position.x, position.y] is Castle;
        }
        
        /// <summary>
        /// Disables frame by position
        /// </summary>
        /// <param name="position"></param>
        public void DisableFrame(IntVector2 position)
        {
            board.GetBoardButton(position).DisableFrame();
        }
        
        /// <summary>
        /// Enables frameby position
        /// </summary>
        /// <param name="position"></param>
        public void EnableFrame(IntVector2 position)
        {
            board.GetBoardButton(position).EnableFrame();
        }
        
        /// <summary>
        /// Rotates board on 180 angles
        /// </summary>
        public void InvertBoard()
        {
            for (var col = 1; col <= width / 2 + Math.Sign(width % 2); col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    var newCol = width - col + 1;
                    var newRow = height - row + 1;
                    SwapItems(col, row, newCol, newRow);
                }
            }
        }

        /// <summary>
        /// Fills tables with army and bonus items from given arrays
        /// </summary>
        /// <param name="items"> Fills table with army items </param>
        /// <param name="bonusItems"> Fills table with bonus items </param>
        public void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems)
        {
            for (var col = 1; col <= width; col++)
            {
                for (var row = 1; row <= height; row++)
                {
                    boardTable[col, row] = items[col, row];
                    bonusTable[col, row] = bonusItems[col, row];
                }
            }
        }

        private void SwapItems(int firstCol, int firstRow, int secondCol, int secondRow)
        {
            var tmp = GetItem(firstCol, firstRow);
            SetItem(firstCol, firstRow, boardTable[secondCol, secondRow]);
            SetItem(secondCol, secondRow, tmp);
            
            var tmp2 = GetBonusItem(firstCol, firstRow);
            SetBonusItem(firstCol, firstRow, bonusTable[secondCol, secondRow]);
            SetBonusItem(secondCol, secondRow, tmp2);
        }

        /// <summary>
        /// Clears tables with army and bonus items
        /// </summary>
        public void Reset()
        {
            for (var row = 1; row <= height; row++)
            {
                for (var col = 1; col <= width; col++)
                {
                    var oldItem = GetItem(col, row);
                    SetItem(col, row, null);
                    if (oldItem != null)
                    {
                        Object.Destroy(oldItem.StoredObject);
                    }
                    
                    var oldBonusItem = GetItem(col, row);
                    SetBonusItem(col, row, null);
                    if (oldBonusItem != null)
                    {
                        Object.Destroy(oldBonusItem.StoredObject);
                    }
                }
            }
            Initialize(width, height);
            board.Reset();
        }

        /// <summary>
        /// Returns cell by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Cell GetCellByPosition(IntVector2 position)
        {
            return cells[position.x, position.y];
        }

        /// <summary>
        /// Returns number of cells
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfCells()
        {
            return GetBoardWidth() * GetBoardHeight();
        }

        /// <summary>
        /// Returns list of board cells
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cell> GetListOfCells()
        {
            return cells.Cast<Cell>().Where(cell => cell != null).ToList();
        }

        /// <summary>
        /// Returns all passes from block
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pass> GetPasses()
        {
            var passes = new List<Pass>();
            foreach (var bonus in bonusTable)
            {
                if (bonus is Pass pass)
                {
                    passes.Add(pass);
                }
            }

            return passes;
        }
    }
}