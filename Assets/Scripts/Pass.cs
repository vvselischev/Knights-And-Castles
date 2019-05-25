using UnityEngine;

namespace Assets.Scripts
{
    public class Pass : BoardStorageItem
    {
        public IntVector2 toBlock { get; }
        public IntVector2 FromPosition { get; set; }
        public IntVector2 ToPosition { get; set; }
        private BoardManager boardManager;

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