using System;
using Assets.Scripts;
using NUnit.Framework;

namespace Editor
{
    public class ArmyTest
    {
        [Test]
        public void TestMergeUserArmies()
        {
            var userArmy1 = new UserArmy(PlayerType.FIRST, new ArmyComposition(3, 2, 5));
            var userArmy2 = new UserArmy(PlayerType.FIRST, new ArmyComposition(4, 1, 5));
            var mergeUserArmies = Army.Merge(userArmy1, userArmy2);
            
            Assert.True(mergeUserArmies.ArmyComposition.Spearmen == 7);
            Assert.True(mergeUserArmies.ArmyComposition.Archers == 3);
            Assert.True(mergeUserArmies.ArmyComposition.Cavalrymen == 10);
            Assert.True(mergeUserArmies.PlayerType == PlayerType.FIRST);
        }
        
        [Test]
        public void TestMergeUserAndNeutralArmies()
        {
            var userArmy = new UserArmy(PlayerType.FIRST, new ArmyComposition(3, 2, 5));
            var neutralArmy = new NeutralFriendlyArmy(new ArmyComposition(4, 1, 5));
            var mergeArmy = Army.Merge(userArmy, neutralArmy);
            
            Assert.True(mergeArmy.ArmyComposition.Spearmen == 7);
            Assert.True(mergeArmy.ArmyComposition.Archers == 3);
            Assert.True(mergeArmy.ArmyComposition.Cavalrymen == 10);
            Assert.True(mergeArmy.PlayerType == PlayerType.FIRST);
        }

        [Test]
        public void TestPerformAction()
        {
            var firstUserArmy = new UserArmy(PlayerType.FIRST, new ArmyComposition(5, 6, 1));
            var secondUserArmy = new UserArmy(PlayerType.SECOND, new ArmyComposition(16, 5, 5));
            var neutralFriendlyArmy = new NeutralFriendlyArmy(new ArmyComposition(12, 15, 17));
            var neutralAggressiveArmy = new NeutralAggressiveArmy(new ArmyComposition(15, 4, 5));

            var battleArmy = secondUserArmy.PerformAction(neutralAggressiveArmy);
            var mergeArmy = neutralFriendlyArmy.PerformAction(firstUserArmy);
            
            Assert.True(battleArmy.PlayerType == PlayerType.SECOND);
            Assert.True(mergeArmy.PlayerType == PlayerType.FIRST);
            Assert.True(mergeArmy.ArmyComposition.Archers == 21);
            Assert.True(mergeArmy.ArmyComposition.Spearmen == 17);
            Assert.True(mergeArmy.ArmyComposition.Cavalrymen == 18);
            Assert.True(battleArmy.ArmyComposition.TotalUnitQuantity() < secondUserArmy.ArmyComposition.TotalUnitQuantity());

            var battleArmy2 = battleArmy.PerformAction(mergeArmy);
            
            Assert.True(battleArmy2.PlayerType == PlayerType.FIRST);
        }

        [Test]
        public void TestSplitIntoEqualParts()
        {
            var firstUserArmy = new UserArmy(PlayerType.FIRST, new ArmyComposition(3, 6, 1));
            var secondUserArmy = new UserArmy(PlayerType.SECOND, new ArmyComposition(16, 5, 5));

            var firstArmyPart = firstUserArmy.SplitIntoEqualParts();
            var secondArmyPart = secondUserArmy.SplitIntoEqualParts();
            
            Assert.True(firstArmyPart.PlayerType == PlayerType.FIRST);
            Assert.True(secondArmyPart.PlayerType == PlayerType.SECOND);
            
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Spearmen - firstUserArmy.ArmyComposition.Spearmen));
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Archers - firstUserArmy.ArmyComposition.Archers));
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Cavalrymen - firstUserArmy.ArmyComposition.Cavalrymen));
            
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Spearmen - secondUserArmy.ArmyComposition.Spearmen));
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Archers - secondUserArmy.ArmyComposition.Archers));
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Cavalrymen - secondUserArmy.ArmyComposition.Cavalrymen));
            
            Assert.True(firstArmyPart.ArmyComposition.Spearmen + firstUserArmy.ArmyComposition.Spearmen == 3);
            Assert.True(firstArmyPart.ArmyComposition.Archers + firstUserArmy.ArmyComposition.Archers == 6);
            Assert.True(firstArmyPart.ArmyComposition.Cavalrymen + firstUserArmy.ArmyComposition.Cavalrymen == 1);
            
            Assert.True(secondArmyPart.ArmyComposition.Spearmen + secondUserArmy.ArmyComposition.Spearmen == 16);
            Assert.True(secondArmyPart.ArmyComposition.Archers + secondUserArmy.ArmyComposition.Archers == 5);
            Assert.True(secondArmyPart.ArmyComposition.Cavalrymen + secondUserArmy.ArmyComposition.Cavalrymen == 5);
        }
    }
}
