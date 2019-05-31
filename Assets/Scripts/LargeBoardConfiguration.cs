namespace Assets.Scripts
{
    public class LargeBoardConfiguration : BoardConfiguration
    {
        public override int BlocksHorizontal { get; } = 2;
        public override int BlocksVertical { get; } = 2;
        public override int BlockWidth { get; } = 8;
        public override int BlockHeight { get; } = 10;
        public override IntVector2[] StartFirstPositions { get; } = 
            {new IntVector2(1, 1), new IntVector2(1, 2), new IntVector2(2,1)};
        public override IntVector2[] StartSecondPositions { get; } = 
            {new IntVector2(8, 10), new IntVector2(8, 9), new IntVector2(7, 10)};
        public override IntVector2[] FirstCastlesBlocks { get; } = {new IntVector2(1, 1)};
        public override IntVector2[] SecondCastlesBlocks { get; } = {new IntVector2(2, 2)};
        public override IntVector2[] FirstCastlesPositions { get; } = {new IntVector2(1, 1)};
        public override IntVector2[] SecondCastlesPositions { get; } = {new IntVector2(8, 10)};
        public override IntVector2 FirstStartBlock { get; } = new IntVector2(1, 1);
        public override IntVector2 SecondStartBlock { get; } = new IntVector2(2, 2);

        public override int PassesNumber { get; } = 8;

        public override IntVector2[] PassesFromBlocks { get; } =
        {
            new IntVector2(1, 1), new IntVector2(1, 1),
            new IntVector2(1, 2), new IntVector2(1, 2),
            new IntVector2(2, 1), new IntVector2(2, 1),
            new IntVector2(2, 2), new IntVector2(2, 2)
        };

        public override IntVector2[] PassesToBlocks { get; } =
        {
            new IntVector2(1, 2), new IntVector2(2, 1),
            new IntVector2(1, 1), new IntVector2(2, 2),
            new IntVector2(1, 1), new IntVector2(2, 2),
            new IntVector2(1, 2), new IntVector2(2, 1)

        };

        public override IntVector2[] PassesFromPositions { get; } =
        {
            new IntVector2(4, 10), new IntVector2(8, 5),
            new IntVector2(4,1), new IntVector2(8, 6),
            new IntVector2(1, 5), new IntVector2(5, 10),
            new IntVector2(1, 6), new IntVector2(5, 1)
        };

        public override IntVector2[] PassesToPositions { get; } =
        {
            new IntVector2(4, 2), new IntVector2(2, 5),
            new IntVector2(4, 9), new IntVector2(2, 6),
            new IntVector2(7, 5), new IntVector2(5, 2),
            new IntVector2(7, 6), new IntVector2(5, 9)
        };
    }
}