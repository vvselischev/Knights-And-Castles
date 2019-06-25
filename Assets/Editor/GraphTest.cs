using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class GraphTest
    {
        [Test]
        public void TestGetDistance()
        {
            var board = new BlockBoardStorage(1, 2, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            
            var pass1 = new Pass(null, null, new IntVector2(1, 2), 
                new IntVector2(1, 1), new IntVector2(2, 2));
            var pass2 = new Pass(null, null, new IntVector2(1, 1),
                new IntVector2(2, 2), new IntVector2(1, 1));
            
            bonusItems[1, 1] = pass1;
            board.FillBlockWithoutCheckeredBoard(new IntVector2(1, 1), items, bonusItems);

            bonusItems[1, 1] = null;
            bonusItems[2, 2] = pass2;
            board.FillBlockWithoutCheckeredBoard(new IntVector2(1, 2), items, bonusItems);
            
            var graph = new Graph(board);
            var cell0 = board.GetBlock(new IntVector2(1, 1)).GetCellByPosition(new IntVector2(1, 2));
            var cell1 = board.GetBlock(new IntVector2(1, 1)).GetCellByPosition(new IntVector2(1, 1));
            var cell2 = board.GetBlock(new IntVector2(1, 2)).GetCellByPosition(new IntVector2(2, 2));

            Assert.True(graph.GetDistance(cell0, cell2) == 1);
            Assert.True(graph.GetDistance(cell0, cell1) == 1);
        }
        
        [Test]
        public void TestGetDistanceInNotConnectedGraph()
        {
            var board = new BlockBoardStorage(1, 2, null);
            
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            board.FillBlockWithoutCheckeredBoard(new IntVector2(1, 1), items, bonusItems);
            board.FillBlockWithoutCheckeredBoard(new IntVector2(1, 2), items, bonusItems);
            
            var graph = new Graph(board);
            var cell0 = board.GetBlock(new IntVector2(1, 1)).GetCellByPosition(new IntVector2(1, 2));
            var cell1 = board.GetBlock(new IntVector2(1, 1)).GetCellByPosition(new IntVector2(1, 1));
            var cell2 = board.GetBlock(new IntVector2(1, 2)).GetCellByPosition(new IntVector2(2, 2));

            Assert.True(graph.GetDistance(cell0, cell2) > 2);
            Assert.True(graph.GetDistance(cell0, cell1) == 1);
        }
    }
}
