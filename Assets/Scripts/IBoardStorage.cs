using System.Collections.Generic;

namespace Assets.Scripts
{
    public interface IBoardStorage
    {
        void DisableBoardButtons();
        void EnableBoardButtons();
        int GetBoardWidth();
        int GetBoardHeight();
        BoardStorageItem GetItem(int positionX, int positionY);
        void SetItem(int col, int row, BoardStorageItem item);
        void SetItem(IntVector2 position, BoardStorageItem item);
        BoardButton GetBoardButton(IntVector2 position);
        BoardStorageItem GetItem(IntVector2 position);
        void DisableFrame(IntVector2 position);
        void EnableFrame(IntVector2 position);
        bool IsCastle(IntVector2 position);
        Castle GetCastle(IntVector2 position);
        BoardStorageItem GetBonusItem(IntVector2 position);
        bool ContainsPlayerArmies(PlayerType playerType);
        void EnableArmies(PlayerType playerType);
        List<Cell> FindPlayerArmies(PlayerType playerType);
        IBoardStorage CreateSimulationStorage();
        BoardStorageItem GetItem(Cell cell);
        void SetItem(Cell cell, BoardStorageItem item);
        int GetDistanceToEnemyCastle(Cell cell, PlayerType playerType);
        IEnumerable<Cell> GetAdjacent(Cell cell);
        List<Cell> FindActivePlayerArmies(PlayerType playerType);
        IntVector2 GetPositionOnBoard(Cell cell);
        SingleBoardStorage GetBlock(Cell cell);
        void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems);
        void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems);
    }
}