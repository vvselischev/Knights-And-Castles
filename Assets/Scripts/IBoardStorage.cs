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
        
        /// <summary>
        /// Sets item by position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        void SetItem(IntVector2 position, BoardStorageItem item);
        
        /// <summary>
        /// Returns board button by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BoardButton GetBoardButton(IntVector2 position);
        
        /// <summary>
        /// Returns army item by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BoardStorageItem GetItem(IntVector2 position);
        
        /// <summary>
        /// Disables frame by position
        /// </summary>
        /// <param name="position"></param>
        void DisableFrame(IntVector2 position);
        
        /// <summary>
        /// Enables frame by position
        /// </summary>
        /// <param name="position"></param>
        void EnableFrame(IntVector2 position);
        
        /// <summary>
        /// Checks that on given position castle is located
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool IsCastle(IntVector2 position);
        
        /// <summary>
        /// Returns castle by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Castle GetCastle(IntVector2 position);
        
        /// <summary>
        /// Returns bonus item by position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BoardStorageItem GetBonusItem(IntVector2 position);
        
        /// <summary>
        /// Checks that board contains at least one army with given player type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        bool ContainsPlayerArmies(PlayerType playerType);
        
        /// <summary>
        /// Sets all armies active
        /// </summary>
        /// <param name="playerType"></param>
        void EnableArmies(PlayerType playerType);
        
        /// <summary>
        /// Returns cells with active armies of given type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        List<Cell> FindPlayerArmies(PlayerType playerType);
        
        /// <summary>
        /// Creates copy of board for UI
        /// </summary>
        /// <returns></returns>
        IBoardStorage CreateSimulationStorage();
        
        /// <summary>
        /// Returns army item by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        BoardStorageItem GetItem(Cell cell);
        
        /// <summary>
        /// Sets army item by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="item"></param>
        void SetItem(Cell cell, BoardStorageItem item);
        
        /// <summary>
        /// Returns distane to enemy castle
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        int GetDistanceToEnemyCastle(Cell cell, PlayerType playerType);
        
        /// <summary>
        /// Returns adjacent cells
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        IEnumerable<Cell> GetAdjacent(Cell cell);
        
        /// <summary>
        /// Returns cells with active player armies of given type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        List<Cell> FindActivePlayerArmies(PlayerType playerType);
        
        /// <summary>
        /// Returns position on board by cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        IntVector2 GetPositionOnBoard(Cell cell);
        
        /// <summary>
        /// Returns block containing given cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        SingleBoardStorage GetBlock(Cell cell);
        
        /// <summary>
        /// Fills board from given arrays
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bonusItems"></param>
        void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems);
        
        /// <summary>
        /// Converts board to arrays
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bonusItems"></param>
        void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems);
    }
}