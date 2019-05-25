using UnityEngine;

namespace Assets.Scripts
{
    public class BoardStorageEmulation
    {
        private ArmyStorageItemEmulation[,] boardStorageEmulation;

        public int Height { get; }

        public int Width { get; }

        public BoardStorageEmulation(IBoardStorage boardStorage)
        {
            Height = boardStorage.GetBoardHeight();
            Width = boardStorage.GetBoardWidth(); 
            boardStorageEmulation = new  ArmyStorageItemEmulation[Width + 1, Height + 1];
            
            for (var i = 1; i <= Width; i++)
            {
                for (var j = 1; j <= Height; j++)
                {
                    boardStorageEmulation[i, j] = new ArmyStorageItemEmulation(boardStorage.GetItem(new IntVector2(i,j)));
                }
            }
        }

        public void SetItem(int x, int y, ArmyStorageItemEmulation item)
        {
            boardStorageEmulation[x, y] = item;
        }

        public ArmyStorageItemEmulation GetItem(int i, int j)
        {
            return boardStorageEmulation[i, j];
        }
    }
}