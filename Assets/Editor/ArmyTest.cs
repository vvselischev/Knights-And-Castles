using System;
using Assets.Scripts;
using NUnit.Framework;
using UnityEngine.VR;

namespace Editor
{
    public class ArmyTest
    {
        [Test]
        public void TestMergeUserArmies()
        {
            var playerType = PlayerType.FIRST;
            var firstSpearmen = 3;
            var secondSpearmen = 4;
            var firstArchers = 2;
            var secondArchers = 1;
            var firstCavalrymen = 5;
            var secondCavalrymen = 5;
            
            var userArmy1 = new UserArmy(playerType, new ArmyComposition(firstSpearmen, firstArchers, firstCavalrymen));
            var userArmy2 = new UserArmy(playerType, new ArmyComposition(secondSpearmen, secondArchers, secondCavalrymen));
            var mergeUserArmies = Army.Merge(userArmy1, userArmy2);
            
            Assert.True(mergeUserArmies.ArmyComposition.Spearmen == firstSpearmen + secondSpearmen);
            Assert.True(mergeUserArmies.ArmyComposition.Archers == firstArchers + secondArchers);
            Assert.True(mergeUserArmies.ArmyComposition.Cavalrymen == firstCavalrymen + secondCavalrymen);
            Assert.True(mergeUserArmies.PlayerType == playerType);
        }
        
        [Test]
        public void TestMergeUserAndNeutralArmies()
        {
            var playerType = PlayerType.FIRST;
            var userSpearmen = 3;
            var neutralSpearmen = 4;
            var userArchers = 2;
            var neutralArchers = 1;
            var userCavalrymen = 5;
            var neutralCavalrymen = 5;
            
            var userArmy = new UserArmy(playerType, new ArmyComposition(userSpearmen, userArchers, userCavalrymen));
            var neutralArmy = new NeutralFriendlyArmy(new ArmyComposition(neutralSpearmen, neutralArchers, neutralCavalrymen));
            var mergeArmy = Army.Merge(userArmy, neutralArmy);
            
            Assert.True(mergeArmy.ArmyComposition.Spearmen == userSpearmen + neutralSpearmen);
            Assert.True(mergeArmy.ArmyComposition.Archers == userArchers + neutralArchers);
            Assert.True(mergeArmy.ArmyComposition.Cavalrymen == userCavalrymen + neutralCavalrymen);
            Assert.True(mergeArmy.PlayerType == playerType);
        }

        [Test]
        public void TestPerformAction()
        {
            var firstPlayerType = PlayerType.FIRST;
            var firstSpearmen = 5;
            var firstArchers = 6;
            var firstCavalrymen = 1;
            var firstUserArmy = new UserArmy(firstPlayerType, new ArmyComposition(firstSpearmen, firstArchers, firstCavalrymen));

            var secondPlayerType = PlayerType.SECOND;
            var secondSpearmen = 16;
            var secondArchers = 5;
            var secondCavalrymen = 5;
            var secondUserArmy = new UserArmy(secondPlayerType, new ArmyComposition(secondSpearmen, secondArchers, secondCavalrymen));
            
            var neutralFriendlySpearmen = 12;
            var neutralFriendlyArchers = 15;
            var neutralFriendlyCavalrymen = 17;
            var neutralFriendlyArmy = new NeutralFriendlyArmy(new ArmyComposition(neutralFriendlySpearmen, neutralFriendlyArchers, neutralFriendlyCavalrymen));

            var neutralAggressiveSpearmen = 15;
            var neutralAggressiveArchers = 4;
            var neutralAggressiveCavalrymen = 5;
            var neutralAggressiveArmy = new NeutralAggressiveArmy(new ArmyComposition(neutralAggressiveSpearmen, neutralAggressiveArchers, neutralAggressiveCavalrymen));

            var battleArmy = secondUserArmy.PerformAction(neutralAggressiveArmy);
            var mergeArmy = neutralFriendlyArmy.PerformAction(firstUserArmy);
            
            Assert.True(battleArmy.PlayerType == secondPlayerType);
            Assert.True(mergeArmy.PlayerType == firstPlayerType);
            Assert.True(mergeArmy.ArmyComposition.Archers == firstArchers + neutralFriendlyArchers);
            Assert.True(mergeArmy.ArmyComposition.Spearmen == firstSpearmen + neutralFriendlySpearmen);
            Assert.True(mergeArmy.ArmyComposition.Cavalrymen == firstCavalrymen + neutralFriendlyCavalrymen);
            Assert.True(battleArmy.ArmyComposition.TotalUnitQuantity() < secondUserArmy.ArmyComposition.TotalUnitQuantity());

            var battleArmy2 = battleArmy.PerformAction(mergeArmy);
            
            Assert.True(battleArmy2.PlayerType == firstPlayerType);
        }

        [Test]
        public void TestSplitIntoEqualParts()
        {
            var firstPlayerType = PlayerType.FIRST;
            var secondPlayerType = PlayerType.SECOND;

            var firstSpearmen = 3;
            var firstArchers = 6;
            var firstCavalrymen = 1;
            var firstUserArmy = new UserArmy(firstPlayerType, new ArmyComposition(firstSpearmen, firstArchers, firstCavalrymen));

            var secondSpearmen = 16;
            var secondArchers = 5;
            var secondCavalrymen = 5;
            var secondUserArmy = new UserArmy(secondPlayerType, new ArmyComposition(secondSpearmen, secondArchers, secondCavalrymen));

            var firstArmyPart = firstUserArmy.SplitIntoEqualParts();
            var secondArmyPart = secondUserArmy.SplitIntoEqualParts();
            
            Assert.True(firstArmyPart.PlayerType == firstPlayerType);
            Assert.True(secondArmyPart.PlayerType == secondPlayerType);
            
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Spearmen - firstUserArmy.ArmyComposition.Spearmen));
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Archers - firstUserArmy.ArmyComposition.Archers));
            Assert.True(1 >= Math.Abs(firstArmyPart.ArmyComposition.Cavalrymen - firstUserArmy.ArmyComposition.Cavalrymen));
            
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Spearmen - secondUserArmy.ArmyComposition.Spearmen));
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Archers - secondUserArmy.ArmyComposition.Archers));
            Assert.True(1 >= Math.Abs(secondArmyPart.ArmyComposition.Cavalrymen - secondUserArmy.ArmyComposition.Cavalrymen));
            
            Assert.True(firstArmyPart.ArmyComposition.Spearmen + firstUserArmy.ArmyComposition.Spearmen == firstSpearmen);
            Assert.True(firstArmyPart.ArmyComposition.Archers + firstUserArmy.ArmyComposition.Archers == firstArchers);
            Assert.True(firstArmyPart.ArmyComposition.Cavalrymen + firstUserArmy.ArmyComposition.Cavalrymen == firstCavalrymen);
            
            Assert.True(secondArmyPart.ArmyComposition.Spearmen + secondUserArmy.ArmyComposition.Spearmen == secondSpearmen);
            Assert.True(secondArmyPart.ArmyComposition.Archers + secondUserArmy.ArmyComposition.Archers == secondArchers);
            Assert.True(secondArmyPart.ArmyComposition.Cavalrymen + secondUserArmy.ArmyComposition.Cavalrymen == secondCavalrymen);
        }
    }
}
