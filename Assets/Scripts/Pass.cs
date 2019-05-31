using UnityEngine;

namespace Assets.Scripts
{
    public class Pass : BoardStorageItem
    {
        public IntVector2 ToBlock { get; }
        public IntVector2 FromPosition { get; set; }
        public IntVector2 ToPosition { get; set; }
        private BoardManager boardManager;

        public void ChangeBlock()
        {
            boardManager.SetActiveBlock(ToBlock);
        }
        
        public void PassArmy(ArmyStorageItem army)
        {
            boardManager.GetCurrentBlock().SetItem(FromPosition, null);
            ChangeBlock();
            boardManager.GetCurrentBlock().SetItem(ToPosition, army);
        }
        
        public Pass(GameObject targetObject, BoardManager boardManager, IntVector2 toBlock, 
            IntVector2 fromPosition, IntVector2 toPosition) : base(targetObject)
        {
            this.boardManager = boardManager;
            ToBlock = toBlock;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }
    }
}