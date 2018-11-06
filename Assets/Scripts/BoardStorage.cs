using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BoardStorage : MonoBehaviour
    {
        public List<BoardStorageItem>[,] boardTable;
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
            boardTable = new List<BoardStorageItem>[board.width + 1, board.height + 1];
            for (int i = 1; i <= board.width; i++)
            {
                for (int j = 1; j <= board.height; j++)
                {
                    boardTable[i, j] = new List<BoardStorageItem>();
                }
            }
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
                    foreach (BoardStorageItem item in boardTable[col, row])
                    {
                        item.BoardButton = targetButton;
                        item.BoardButton.Initialize(newCol, newCol);
                    }
                    foreach (BoardStorageItem item in boardTable[newCol, newRow])
                    {
                        item.BoardButton = board.BoardButtons[col, row].GetComponent<BoardButton>();
                        item.BoardButton.Initialize(col, row);
                    }

                    List<BoardStorageItem> tmp = boardTable[col, row];
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