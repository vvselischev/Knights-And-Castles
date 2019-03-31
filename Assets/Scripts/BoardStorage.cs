using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class BoardStorage : MonoBehaviour
    {
        public BoardStorageItem[,] boardTable;
        public CheckeredButtonBoard board;
        public BoardStorageItem[,] bonusTable;
        
        void Awake()
        {
            Initialize();  
        }

        public void Initialize()
        {
            boardTable = new BoardStorageItem[board.width + 1, board.height + 1];
            bonusTable = new BoardStorageItem[board.width + 1, board.height + 1];
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
            return board.BoardButtons[positionX, positionY].GetComponent<BoardButton>();
        }
        public BoardButton GetBoardButton(IntVector2 position)
        {
            return GetBoardButton(position.x, position.y);
        }

        public void AddCastle(IntVector2 position, Castle castle)
        {
            bonusTable[position.x, position.y] = castle;
        }

        public Castle GetCastle(IntVector2 position)
        {
            return bonusTable[position.x, position.y] as Castle;
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
                        Destroy(oldItem.StoredObject);
                    }
                    
                    var oldBonusItem = GetItem(col, row);
                    SetBonusItem(col, row, null);
                    if (oldBonusItem != null)
                    {
                        Destroy(oldBonusItem.StoredObject);
                    }
                }
            }
            Initialize();
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
    }
}