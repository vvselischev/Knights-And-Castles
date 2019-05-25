namespace Assets.Scripts
{
    public interface IBoardStorage
    {
        void DisableBoardButtons();
        void EnableBoardButtons();
        void InvertBoard();
        void Fill(BoardStorageItem[,] items, BoardStorageItem[,] bonusItems);
        void ConvertToArrays(out BoardStorageItem[,] items, out BoardStorageItem[,] bonusItems);
        int GetBoardWidth();
        int GetBoardHeight();
        BoardStorageItem GetItem(int positionX, int positionY);
        void SetItem(int col, int row, BoardStorageItem item);
        void SetItem(IntVector2 position, BoardStorageItem item);
        BoardButton GetBoardButton(IntVector2 position);
        BoardStorageItem GetItem(IntVector2 position);
        BoardStorageItem GetBonusItem(int positionX, int positionY);
        void DisableFrame(IntVector2 position);
        void EnableFrame(IntVector2 position);
        bool IsCastle(IntVector2 position);
        Castle GetCastle(IntVector2 position);
        BoardStorageItem GetBonusItem(IntVector2 position);
        bool ContainsPlayerArmies(PlayerType playerType);
        void EnableArmies(PlayerType playerType);
    }
}