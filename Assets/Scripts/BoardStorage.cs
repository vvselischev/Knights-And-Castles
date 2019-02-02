using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BoardStorage : MonoBehaviour
    {
        public BoardStorageItem[,] boardTable;
        public CheckeredButtonBoard board;
        private static BoardStorage instance;

        public static BoardStorage GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            boardTable = new BoardStorageItem[board.width + 1, board.height + 1];
        }

        public BoardStorageItem GetItem(int positionX, int positionY)
        {
            return boardTable[positionX, positionY];
        }

        public void SetItem(int positionX, int positionY, BoardStorageItem item)
        {
            boardTable[positionX, positionY] = item;
            BoardButton targetButton = GetBoardButton(positionX, positionY);
            if (item != null)
            {
                boardTable[positionX, positionY].StoredObject.transform.position = 
                    targetButton.transform.position;
            }
        }
        public BoardStorageItem GetItem(Vector2 position)
        {
            return GetItem((int)position.x, (int)position.y);
        }

        public void SetItem(Vector2 position, BoardStorageItem item)
        {
            SetItem((int)position.x, (int)position.y, item);
        }

        public BoardButton GetBoardButton(int positionX, int positionY)
        {
            return board.BoardButtons[positionX, positionY].GetComponent<BoardButton>();
        }
        public BoardButton GetBoardButton(Vector2 position)
        {
            return GetBoardButton((int)position.x, (int)position.y);
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
        }

        public void RemoveBoardItem(BoardButton boardButton)
        {
            
        }

        public void Reset()
        {
            for (int row = 1; row <= board.height; row++)
            {
                for (int col = 1; col <= board.width - row + 1; col++)
                {
                    SetItem(col, row, null);
                }
            }
        }
    }
}