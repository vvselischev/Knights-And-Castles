namespace  Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Board settings for small board mode.
    /// </summary>
    public class SmallBoardConfiguration : BoardConfiguration
    {
        public override int BlocksHorizontal { get; } = 1;
        public override int BlocksVertical { get; } = 1;
        public override int BlockWidth { get; } = 8;
        public override int BlockHeight { get; } = 10;

        public override IntVector2[] StartFirstPositions { get; } =
            {new IntVector2(2, 1), new IntVector2(1, 2)};

        public override IntVector2[] StartSecondPositions { get; } =
            {new IntVector2(7, 10), new IntVector2(8, 9)};

        public override IntVector2[] FirstCastlesBlocks { get; } = {new IntVector2(1, 1)};
        public override IntVector2[] SecondCastlesBlocks { get; } = {new IntVector2(1, 1)};
        
        public override IntVector2[] FirstCastlesPositions { get; } = {new IntVector2(1, 1)};
        public override IntVector2[] SecondCastlesPositions { get; } = {new IntVector2(8, 10)};

        public override IntVector2 FirstStartBlock { get; } = new IntVector2(1, 1);
        public override IntVector2 SecondStartBlock { get; } = new IntVector2(1, 1);
        public override int PassesNumber { get; } = 0;
        public override IntVector2[] PassesFromBlocks { get; } = new IntVector2[0];
        public override IntVector2[] PassesToBlocks { get; } = new IntVector2[0];
        public override IntVector2[] PassesFromPositions { get; } = new IntVector2[0];
        public override IntVector2[] PassesToPositions { get; } = new IntVector2[0];
    }
}