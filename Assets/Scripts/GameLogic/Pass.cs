using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a pass between blocks on board.
    /// </summary>
    public class Pass : BoardStorageItem
    {
        /// <summary>
        /// The coordinate of the block that contains the exit of the pass.
        /// </summary>
        public IntVector2 ToBlock { get; }
        /// <summary>
        /// The coordinate inside the block of the entrance of the pass.
        /// </summary>
        public IntVector2 FromPosition { get; set; }
        /// <summary>
        /// The coordinate inside the ToBlock of the exit of the pass.
        /// </summary>
        public IntVector2 ToPosition { get; set; }
        //We do not need FromBlock
        
        /// <summary>
        /// To change the current block to the new one that contains the exit.
        /// </summary>
        private BoardManager boardManager;

        /// <summary>
        /// Changes block to the one, that contains the exit of this pass.
        /// </summary>
        public void ChangeBlock()
        {
            boardManager.SetActiveBlock(ToBlock);
        }
        
        /// <summary>
        /// Transfers the given army and changes block.
        /// Removes the given army from the current block, changes the block and places the army to the new block.
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