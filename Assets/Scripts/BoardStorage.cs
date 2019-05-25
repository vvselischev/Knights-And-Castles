using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class BoardStorage : IBoardStorage
    {
        private BoardStorageItem[,] boardTable;
        private BoardStorageItem[,] bonusTable;
        
        //TODO: remove this dependency?
        private CheckeredButtonBoard board;
        
        public BoardStorage(int width, int height, CheckeredButtonBoard board)
        {
            this.board = board;
            Initialize(width, height);  
        }

        private void Initialize(int width, int height)
        {
            boardTable = new BoardStorageItem[width + 1, height + 1];
            bonusTable = new BoardStorageItem[width + 1, height + 1];
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
            for (int col = 1; col <= board.width; col++)
            {
                for (int row = 1; row <= board.height; row++)
                {
                    boardTable[col, row]?.StoredObject.SetActive(active);
                    bonusTable[col, row]?.StoredObject.SetActive(active);
                }
            }
        }

        public int GetBoardHeight()
        {
            return board.height;
        }

        public void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems)
        {
            for (int col = 1; col <= board.width; col++)
            {
                for (int row = 1; row <= board.height; row++)
                {
                    boardTable[col, row] = items[col, row];
                    bonusTable[col, row] = items[col, row];
                }
            }
        }

        public void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems)
        {
            items = new BoardStorageItem[board.width + 1, board.height + 1];
            bonusItems = new BoardStorageItem[board.width + 1, board.height + 1];
            for (int col = 1; col <= board.width; col++)
            {
                for (int row = 1; row <= board.height; row++)
                {
                    items[col, row] = boardTable[col, row];
                    bonusItems[col, row] = bonusItems[col, row];
                }
            }
        }

        public int GetBoardWidth()
        {
            return board.width;
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
            if (item != null)
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
            return FindPlayerArmies().ContainsKey(playerType);
        }

        public void EnableArmies(PlayerType playerType)
        {
            for (int i = 1; i <= GetBoardHeight(); i++)
            {
                for (int j = 1; j <= GetBoardWidth(); j++)
                {
                    if (GetItem(j, i) is ArmyStorageItem)
                    {
                        Army army = ((ArmyStorageItem)GetItem(j, i)).Army;
                        if (army.playerType == playerType)
                        {
                            (army as UserArmy).SetActive();
                        }
                    }
                }
            }
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
            for (int col = 1; col <= board.width / 2 + Math.Sign(board.width % 2); col++)
            {
                for (int row = 1; row <= board.height; row++)

                {
                    int newCol = board.width - col + 1;
                    int newRow = board.height - row + 1;

                    SwapItems(col, row, newCol, newRow);
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
            for (int row = 1; row <= board.height; row++)
            {
                for (int col = 1; col <= board.width; col++)
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
            Initialize(board.width, board.height);
            board.Reset();
        }


        //TODO: cache it to make effective
        public Dictionary<PlayerType, List<IntVector2>> FindPlayerArmies()
        {
            var playerArmyPositions = new Dictionary<PlayerType, List<IntVector2>>();

            for (int i = 1; i <= board.width; i++)
            {
                for (int j = 1; j <= board.height; j++)
                {
                    if (GetItem(i, j) != null)
                    {
                        var item = GetItem(i, j);
                        if (item is ArmyStorageItem)
                        {
                            var army = (item as ArmyStorageItem).Army;
                            if (!playerArmyPositions.ContainsKey(army.playerType))
                            {
                                playerArmyPositions.Add(army.playerType, new List<IntVector2>());
                            }
                            playerArmyPositions[army.playerType].Add(new IntVector2(i, j));
                        }
                    }
                }
            }

            return playerArmyPositions;
        }

        public IEnumerable<Pass> GetPasses()
        {
            var passes = new List<Pass>();
            for (int i = 1; i <= board.width; i++)
            {
                for (int j = 1; j <= board.height; j++)
                {
                    var item = GetBonusItem(i, j);
                    if (item is Pass)
                    {
                        passes.Add(item as Pass);
                    }
                }
            }

            return passes;
        }
    }
}