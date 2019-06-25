using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Editor
{
    public class BlockBoardStorageTest
    {
        [Test]
        public void TestGetPasses()
        {
            var board = new BlockBoardStorage(1, 1, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            
            var pass1 = new Pass(null, null, new IntVector2(1, 1), 
                new IntVector2(1, 1), new IntVector2(2, 2));
            var pass2 = new Pass(null, null, new IntVector2(1, 1),
                new IntVector2(2, 2), new IntVector2(1, 1));
            
            bonusItems[1, 1] = pass1;
            bonusItems[2, 2] = pass2;
            
            board.FillBlockForTesting(new IntVector2(1, 1), items, bonusItems);

            var passes = board.GetPassesAsFromToCells();
            Assert.True(passes.Count() == 2);
        }

        [Test]
        public void TestGetUserArmies()
        {
            var board = new BlockBoardStorage(1, 1, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            
            var item1 = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            var item2 = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            
            items[1, 1] = item1;
            items[2, 2] = item2;
            
            board.FillBlockForTesting(new IntVector2(1, 1), items, bonusItems);

            var foundItems = board.FindPlayerArmies(PlayerType.FIRST);
            
            Assert.True(foundItems.Count == 2);
        }

        [Test]
        public void TestGetOpponentPlayerType()
        {
            var first = PlayerType.FIRST;
            var second = PlayerType.SECOND;
            
            Assert.True(BlockBoardStorage.GetOpponentPlayerType(first) == second);
            Assert.True(BlockBoardStorage.GetOpponentPlayerType(second) == first);
        }
        
        [Test]
        public void TestGetUserArmiesWithoutArmies()
        {
            var board = new BlockBoardStorage(1, 1, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            
            var item1 = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            var item2 = new ArmyStorageItem(new UserArmy(PlayerType.FIRST, null), null);
            
            items[1, 1] = item1;
            items[2, 2] = item2;
            
            board.FillBlockForTesting(new IntVector2(1, 1), items, bonusItems);

            var foundItems = board.FindPlayerArmies(PlayerType.SECOND);
            
            Assert.True(foundItems.Count == 0);
        }

        [Test]
        public void TestBoardWithTwoBlocksGetUserArmies()
        {
            var board = new BlockBoardStorage(1, 2, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            var playerType = PlayerType.FIRST;
            
            var item1 = new ArmyStorageItem(new UserArmy(playerType, null), null);
            var item2 = new ArmyStorageItem(new UserArmy(playerType, null), null);
            var item3 = new ArmyStorageItem(new UserArmy(playerType, null), null);
            var item4 = new ArmyStorageItem(new UserArmy(playerType, null), null);
            
            items[1, 1] = item1;
            items[2, 2] = item2;
            
            board.FillBlockForTesting(new IntVector2(1, 1), items, bonusItems);
            
            items[1, 1] = item3;
            items[2, 2] = item4;
            
            board.FillBlockForTesting(new IntVector2(1, 2), items, bonusItems);

            var armies = board.FindPlayerArmies(playerType);
            Assert.True(armies.Count == 4);
        }
        
        [Test]
        public void TestBoardWithTwoBlocksGetPasses()
        {
            var board = new BlockBoardStorage(1, 2, null);
            var items = new BoardStorageItem[3, 3];
            var bonusItems = new BoardStorageItem[3, 3];
            
            var pass1 = new Pass(null, null, new IntVector2(1, 2), 
                new IntVector2(1, 1), new IntVector2(2, 2));
            var pass2 = new Pass(null, null, new IntVector2(1, 2),
                new IntVector2(2, 2), new IntVector2(1, 1));
            var pass3 = new Pass(null, null, new IntVector2(1, 1), 
                new IntVector2(1, 1), new IntVector2(2, 2));
            var pass4 = new Pass(null, null, new IntVector2(1, 1),
                new IntVector2(2, 2), new IntVector2(1, 1));
            
            bonusItems[1, 1] = pass1;
            bonusItems[2, 2] = pass2;
            
            board.FillBlockForTesting(new IntVector2(1, 1), items, bonusItems);
            
            bonusItems[1, 1] = pass3;
            bonusItems[2, 2] = pass4;
            
            board.FillBlockForTesting(new IntVector2(1, 2), items, bonusItems);

            var passes = board.GetPassesAsFromToCells();
            Assert.True(passes.Count() == 4);
        }
    }
}
