using UnityEngine;

namespace Assets.Scripts
{
    public class Pass : BoardStorageItem
    {
        private IntVector2 toBlock;
        public IntVector2 FromPosition { get; }
        public IntVector2 ToPosition { get; }
        public BoardManager boardManager;

        public void ChangeBlock()
        {
            Debug.Log("Change block without army");
            boardManager.SetActiveBlock(toBlock);
        }
        
        public void PassArmy(ArmyStorageItem army)
        {
            Debug.Log("Pass: " + FromPosition + " -> " + ToPosition);
            boardManager.GetCurrentBlock().SetItem(FromPosition, null);
            ChangeBlock();
            Debug.Log("Current block: " + boardManager.BoardStorage.GetCurrentBlockPosition());
            boardManager.GetCurrentBlock().SetItem(ToPosition, army);
        }
        
        public Pass(GameObject targetObject, BoardManager boardManager, IntVector2 toBlock, 
            IntVector2 fromPosition, IntVector2 toPosition) : base(targetObject)
        {
            this.boardManager = boardManager;
            this.toBlock = toBlock;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }
    }
}