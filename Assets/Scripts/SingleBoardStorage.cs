﻿using System;
using System.Collections.Generic;
using UnityEngine.Experimental.PlayerLoop;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class SingleBoardStorage : IBoardStorage
    {
        private BoardStorageItem[,] boardTable;
        private BoardStorageItem[,] bonusTable;
        private Cell[,] cells;
        private Dictionary<Cell, IntVector2> indexByCell = new Dictionary<Cell, IntVector2>();
        private int width;
        private int height;
        
        //TODO: remove this dependency?
        private CheckeredButtonBoard board;
        
        public SingleBoardStorage(int width, int height, CheckeredButtonBoard board)
        {
            this.board = board;
            Initialize(width, height);  
        }

        private void Initialize(int width, int height)
        {
            this.width = width;
            this.height = height;
            boardTable = new BoardStorageItem[width + 1, height + 1];
            bonusTable = new BoardStorageItem[width + 1, height + 1];
            cells = new Cell[width + 1, height + 1];
            CreateCells();
        }

        private void CreateCells()
        {
            for (int i = 1; i <= width; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    var cell = new Cell();
                    indexByCell.Add(cell, new IntVector2(i, j));
                    cells[i, j] = cell;
                }
            }
        }

        public void Activate()
        {
            SetAllItemsActive(true);
        }

        public void Deactivate()
        {
            SetAllItemsActive(false);
        }

        private void SetAllItemsActive(bool active)
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    boardTable[col, row]?.StoredObject.SetActive(active);
                    bonusTable[col, row]?.StoredObject.SetActive(active);
                }
            }
        }

        public int GetBoardHeight()
        {
            return height;
        }

        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems)
        {
            items = new BoardStorageItem[width + 1, height + 1];
            bonusItems = new BoardStorageItem[width + 1, height + 1];
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
                {
                    items[col, row] = boardTable[col, row];
                    bonusItems[col, row] = bonusItems[col, row];
                }
            }
        }

        public int GetBoardWidth()
        {
            return width;
        }
        
        public void EnableBoardButtons()
        {
            board.EnableBoard();
        }

        public void DisableBoardButtons()
        {
            board.DisableBoard();
        }
        
        public BoardStorageItem GetItem(int positionX, int positionY)
        {
            return boardTable[positionX, positionY];
        }

        public void SetItem(int positionX, int positionY, BoardStorageItem item)
        {
            SetItem(positionX, positionY, item, boardTable);
        }

        public BoardStorageItem GetBonusItem(int positionX, int positionY)
        {
            return bonusTable[positionX, positionY];
        }

        public void SetBonusItem(int positionX, int positionY, BoardStorageItem item)
        {
            SetItem(positionX, positionY, item, bonusTable);
        }
        
        private void SetItem(int positionX, int positionY, BoardStorageItem item, BoardStorageItem[,] table)
        {
            table[positionX, positionY] = item;
            BoardButton targetButton = GetBoardButton(positionX, positionY);
            if (item != null && table[positionX, positionY].StoredObject != null)
            {
                table[positionX, positionY].StoredObject.transform.position = 
                    targetButton.transform.position;
            }
        }
        
        public BoardStorageItem GetItem(IntVector2 position)
        {
            return GetItem(position.x, position.y);
        }

        public void SetItem(IntVector2 position, BoardStorageItem item)
        {
            SetItem(position.x, position.y, item);
        }

        public BoardButton GetBoardButton(int positionX, int positionY)
        {
            return GetBoardButton(new IntVector2(positionX, positionY));
        }
        public BoardButton GetBoardButton(IntVector2 position)
        {
            return board.GetBoardButton(position);
        }

        public void AddCastle(IntVector2 position, Castle castle)
        {
            bonusTable[position.x, position.y] = castle;
        }

        public Castle GetCastle(IntVector2 position)
        {
            return bonusTable[position.x, position.y] as Castle;
        }

        public BoardStorageItem GetBonusItem(IntVector2 position)
        {
            return bonusTable[position.x, position.y];
        }

        public bool ContainsPlayerArmies(PlayerType playerType)
        {
            return FindPlayerArmies(playerType).Count > 0;
        }

        public void EnableArmies(PlayerType playerType)
        {
            for (int i = 1; i <= GetBoardHeight(); i++)
            {
                for (int j = 1; j <= GetBoardWidth(); j++)
                {
                    var item = GetItem(j, i) as ArmyStorageItem;
                    if (item != null)
                    {
                        var userArmy = item.Army as UserArmy;
                        if (item.Army.playerType == playerType)
                        {
                            userArmy?.SetActive();
                        }
                    }
                }
            }
        }

        public List<Cell> FindPlayerArmies(PlayerType playerType)
        {
            var cellsWithArmies = new List<Cell>();
            foreach (var dictionaryEntry in indexByCell)
            {
                var boardItem = GetItem(dictionaryEntry.Value);
                if (boardItem is ArmyStorageItem)
                {
                    var item = boardItem as ArmyStorageItem;
                    if (item.Army.playerType == playerType)
                    {
                        cellsWithArmies.Add(dictionaryEntry.Key);
                    }
                }
            }
    
            return cellsWithArmies;
        }

        public IBoardStorage CreateSimulationStorage()
        {
            var simulationStorage = new SingleBoardStorage(GetBoardWidth(), GetBoardHeight(), board)
            {
                cells = cells, indexByCell = indexByCell, bonusTable = bonusTable
            };

            for (int col = 1; col <= GetBoardWidth(); col++)
            {
                for (int row = 1; row <= GetBoardHeight(); row++)
                {
                    var item = boardTable[col, row];
                    if (item is ArmyStorageItem)
                    {
                        var storageItem = item as ArmyStorageItem;
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

        public bool ContainsCell(Cell cell)
        {
            return indexByCell.ContainsKey(cell);
        }

        public BoardStorageItem GetItem(Cell cell)
        {
            if (ContainsCell(cell))
            {
                return GetItem(indexByCell[cell]);
            }

            return null;
        }

        public void SetItem(Cell cell, BoardStorageItem item)
        {
            SetItem(indexByCell[cell], item);
        }

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

        public List<Cell> FindCastle(PlayerType playerType)
        {
            var castlePosition = GetCastlePosition(playerType);

            if (bonusTable[castlePosition.x, castlePosition.y] is Castle)
            {
                return new List<Cell> {cells[castlePosition.x, castlePosition.y]};
            }

            return null;
        }


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

        public IntVector2 GetPositionOnBoard(Cell cell)
        {
            if (indexByCell.ContainsKey(cell))
            {
                return indexByCell[cell];
            }

            return null;
        }

        public SingleBoardStorage GetBlock(Cell cell)
        {
            return this;
        }

        private bool IsActiveArmyInCell(Cell cell)
        {
            var item = GetItem(indexByCell[cell]) as ArmyStorageItem;
            return item?.Army is UserArmy army && army.IsActive();
        }

        public bool IsCastle(IntVector2 position)
        {
            return bonusTable[position.x, position.y] is Castle;
        }
        
        public void DisableFrame(IntVector2 position)
        {
            board.GetBoardButton(position).DisableFrame();
        }
        
        public void EnableFrame(IntVector2 position)
        {
            board.GetBoardButton(position).EnableFrame();
        }
        
        public void InvertBoard()
        {
            for (int col = 1; col <= width / 2 + Math.Sign(width % 2); col++)
            {
                for (int row = 1; row <= height; row++)

                {
                    int newCol = width - col + 1;
                    int newRow = height - row + 1;

                    SwapItems(col, row, newCol, newRow);
                }
            }
        }

        public void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems)
        {
            for (int col = 1; col <= width; col++)
            {
                for (int row = 1; row <= height; row++)
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

        public void Reset()
        {
            for (int row = 1; row <= height; row++)
            {
                for (int col = 1; col <= width; col++)
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

        public Cell GetCellByPosition(IntVector2 position)
        {
            return cells[position.x, position.y];
        }

        public int GetNumberOfCells()
        {
            return GetBoardWidth() * GetBoardHeight();
        }

        public List<Cell> GetListOfCells()
        {
            var listOfCells = new List<Cell>();
            foreach (var cell in cells)
            {
                if (cell != null)
                {
                    listOfCells.Add(cell);
                }
            }

            return listOfCells;
        }

        public List<Pass> GetPasses()
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