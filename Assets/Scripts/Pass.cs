using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a pass between blocks on board.
    /// </summary>
    public class Pass : BoardStorageItem
    {
        public IntVector2 ToBlock { get; }
        public IntVector2 FromPosition { get; set; }
        public IntVector2 ToPosition { get; set; }
        private BoardManager boardManager;

        /// <summary>
        /// Changes block to the one, containing the exit of this pass.
        /// </summary>
        public void ChangeBlock()
        {
            boardManager.SetActiveBlock(ToBlock);
        }
        
        /// <summary>
        /// Transfers the given army and changes block.
        /// </summary>
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