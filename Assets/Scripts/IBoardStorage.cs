using System.Collections.Generic;

namespace Assets.Scripts
{
    public interface IBoardStorage
    {
        /// <summary>
        /// Disables board buttons
        /// </summary>
        void DisableBoardButtons();
        
        /// <summary>
        /// Unables board buttons
        /// </summary>
        void EnableBoardButtons();
        
        /// <summary>
        /// Returns board width 
        /// </summary>
        /// <returns></returns>
        int GetBoardWidth();
        
        /// <summary>
        /// Returns board height
        /// </summary>
        /// <returns></returns>
        int GetBoardHeight();
        
        /// <summary>
        /// Returns army item by position
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns></returns>
        BoardStorageItem GetItem(int positionX, int positionY);
        
        /// <summary>
        /// Sets army item by position
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="item"></param>
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