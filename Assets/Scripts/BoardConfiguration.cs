namespace Assets.Scripts
{
    public abstract class BoardConfiguration
    {
        public abstract int BlocksHorizontal { get; }
        public abstract int BlocksVertical { get; }

        public abstract int BlockWidth { get; }
        public abstract int BlockHeight { get; }
        
        public abstract IntVector2[] StartFirstPositions { get; }
        public abstract IntVector2[] StartSecondPositions { get; }

        public abstract IntVector2[] FirstCastlesBlocks { get; }
        public abstract IntVector2[] SecondCastlesBlocks { get; }

        public abstract IntVector2[] FirstCastlesPositions { get; }
        public abstract IntVector2[] SecondCastlesPositions { get; }
        
        public abstract IntVector2 FirstStartBlock { get; }
        public abstract IntVector2 SecondStartBlock { get; }

        public abstract int PassesNumber { get; }
        public abstract IntVector2[] PassesFromBlocks { get; }
        
        public abstract IntVector2[] PassesToBlocks { get; }
        
        public abstract IntVector2[] PassesFromPositions { get; }
        
        //It is be more convenient for user to be placed next to the pass, but not on the same cell.
        public abstract IntVector2[] PassesToPositions { get; }
    }
}