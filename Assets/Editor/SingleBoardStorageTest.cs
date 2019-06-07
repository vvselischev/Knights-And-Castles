using System.Linq;
using Assets.Scripts;
using NUnit.Framework;

namespace Editor
{
    public class SingleBoardStorageTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestIndexByCell()
        {
            var board = new SingleBoardStorage(2, 3, null);


            for (int i = 1; i <= board.GetBoardWidth(); i++)
            {
                for (int j = 1; j <= board.GetBoardHeight(); j++)
                {
                    var testedPosition = new IntVector2(i, j);
                    var cell = board.GetCellByPosition(testedPosition);
                    var position = board.GetPositionOnBoard(cell);
                    
                    Assert.True(position.Equals(testedPosition));
                }
            }
        }

        [Test]
        public void TestFindPlayerArmies()
        {
            int size = 3;
            var board = new SingleBoardStorage(size, size, null);
            var bonusTable = new BoardStorageItem[size + 1, size + 1];
            var boardTable = new BoardStorageItem[size + 1, size + 1];

            boardTable[2, 2] = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            boardTable[1, 2] = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            
            board.Fill(boardTable, bonusTable);
            
            Assert.True(board.ContainsPlayerArmies(PlayerType.FIRST));
            Assert.False(board.ContainsPlayerArmies(PlayerType.SECOND));

            var armies = board.FindPlayerArmies(PlayerType.FIRST);

            Assert.True(armies.Contains(board.GetCellByPosition(new IntVector2(1, 2))));
            Assert.True(armies.Contains(board.GetCellByPosition(new IntVector2(2, 2))));
            Assert.True(armies.Count == 2);
        }

        [Test]
        public void TestGetPasses()
        {
            int size = 3;
            var board = new SingleBoardStorage(size, size, null);
            var bonusTable = new BoardStorageItem[size + 1, size + 1];
            var boardTable = new BoardStorageItem[size + 1, size + 1];
            var pass = new Pass(null, null, null, null, null);

            bonusTable[2, 2] = pass;
            board.Fill(boardTable, bonusTable);
            
            Assert.True(board.GetPasses().Count() == 1);
            Assert.AreSame(board.GetPasses().First(), pass);
        }

        [Test]
        [Ignore("Visualization connected with board inversion")]
        public void TestInvertBoard()
        {
            int width = 3;
            int height = 2;
            
            var board = new SingleBoardStorage(width, height, null);
            var bonusTable = new BoardStorageItem[width + 1, height + 1];
            var boardTable = new BoardStorageItem[width + 1, height + 1];
            var pass = new Pass(null, null, null, null, null);
            var item = new ArmyStorageItem(null, null);

            bonusTable[2, 1] = pass;
            boardTable[3, 2] = item;
            
            board.Fill(boardTable, bonusTable);
            board.InvertBoard();
            
            Assert.AreSame(board.GetItem(3, 2), null);
            Assert.AreSame(board.GetBonusItem(2, 1), null);
            Assert.AreSame(board.GetItem(2, 2), item);
            Assert.AreSame(board.GetBonusItem(1, 1), pass);
        }
    }
}
