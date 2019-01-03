using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BoardStorage : MonoBehaviour
    {
        public BoardStorageItem[,] boardTable;
        public CheckeredBoard board;
        private static BoardStorage instance;
        public ControllerManager controllerManager;

        public static BoardStorage GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            boardTable = new BoardStorageItem[board.width + 1, board.height + 1];
        }

        public BoardStorageItem GetItem(Vector2 position)
        {
            return boardTable[(int)position.x, (int)position.y];
        }

        public void SetItem(Vector2 position, BoardStorageItem item)
        {
            boardTable[(int)position.x, (int)position.y] = item;
        }

        public BoardButton GetBoardButton(Vector2 position)
        {
            return board.BoardButtons[(int)position.x, (int)position.y].GetComponent<BoardButton>();
        }

        public void InvertBoard()
        {
            for (int row = 1; row <= board.height; row++)
            {
                for (int col = 1; col <= board.width - row + 1; col++)
                {
                    if ((row + col == board.height + 1) && (row > board.height / 2))
                    {
                        continue;
                    }
                    int newCol = board.width - col + 1;
                    int newRow = board.height - row + 1;
                    BoardButton targetButton = board.BoardButtons[newCol, newRow].GetComponent<BoardButton>();
                    
                    BoardStorageItem item = boardTable[col, row];
                    item.BoardButton = targetButton;
                    item.BoardButton.Initialize(newCol, newCol);
                    
                    item = boardTable[newCol, newRow];
                    item.BoardButton = board.BoardButtons[col, row].GetComponent<BoardButton>();
                    item.BoardButton.Initialize(col, row);


                    var tmp = boardTable[col, row];
                    boardTable[col, row] = boardTable[newCol, newRow];
                    boardTable[newCol, newRow] = tmp;
                }
            }
        }

        public void RemoveBoardItem(BoardButton boardButton)
        {
            
        }

        public void Reset()
        {
        }
    }
}