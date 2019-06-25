using System;
using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace Editor
{
    public class ArmyCompositionTest
    {
        [Test]
        public void TestArmyPower()
        {
            // Number of each unit in army
            var spearmen = 3;
            var archers = 6;
            var cavalrymen = 9;
            var first = new ArmyComposition(spearmen, archers, cavalrymen);

            spearmen = 4;
            archers = 8;
            cavalrymen = 12;
            var second = new ArmyComposition(spearmen, archers, cavalrymen);
            Assert.True(first.ArmyPower() < second.ArmyPower());
        }

        [Test]
        public void TestArmyPowerWithExperience()
        {
            var spearmen = 5;
            var archers = 1;
            var cavalrymen = 0;
            var experience = 2;
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            experience = 3;
            var second = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            Assert.True(first.ArmyPower() < second.ArmyPower());
        }

        [Test]
        public void TestIsFirstWinner()
        {
            var spearmen = 6;
            var archers = 8;
            var cavalrymen = 10;
            var experience = 2;
            
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            spearmen = 8;
            archers = 18;
            var second = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            experience = 3;
            var third = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            
            Assert.False(ArmyComposition.IsFirstWinner(first, second));
            Assert.True(ArmyComposition.IsFirstWinner(third, second));
        }

        [Test]
        public void TestTotalUnitQuality()
        {
            var spearmen = 6;
            var archers = 8;
            var cavalrymen = 10;
            var experience = 2;
            
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            Assert.True(first.TotalUnitQuantity() == spearmen + archers + cavalrymen);
            
            spearmen = 8;
            archers = 18;
            cavalrymen = 10;
            
            var second = new ArmyComposition(spearmen, archers, cavalrymen);
            Assert.True(second.TotalUnitQuantity() == spearmen + archers + cavalrymen);
        }

        [Test]
        public void TestDeleteArmyPart()
        {
            var spearmen = 45;
            var archers = 32;
            var cavalrymen = 12;
            var experience = 4;
            var armyComposition = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            var deletingSpearmen = 15;
            var deletingArchers = 22;
            var deletingCavalrymen = 3;
            armyComposition.DeleteArmyPart(deletingSpearmen, deletingArchers, deletingCavalrymen);

            Assert.True(spearmen - deletingSpearmen == armyComposition.Spearmen);
            Assert.True(archers - deletingArchers == armyComposition.Archers);
            Assert.True(cavalrymen - deletingCavalrymen == armyComposition.Cavalrymen);
        }

        [Test]
        public void TestMerge()
        {
            var firstSpearmen = 45;
            var firstArchers = 32;
            var firstCavalrymen = 12;
            var experience = 4;
            var first = new ArmyComposition(firstSpearmen, firstArchers, firstCavalrymen, experience);

            var secondSpearmen = 43;
            var secondArchers = 4;
            var secondCavalrymen = 31;
            var second = new ArmyComposition(secondSpearmen, secondArchers, secondCavalrymen);
            var result = ArmyComposition.Merge(first, second);
            
            Assert.True(result.Spearmen ==  firstSpearmen + secondSpearmen);
            Assert.True(result.Archers == firstArchers + secondArchers);
            Assert.True(result.Cavalrymen == firstCavalrymen + secondCavalrymen);
        }
        
        [Test]
        public void TestMergeBigArmies()
        {
            var firstSpearmen = 4512;
            var firstArchers = 3245;
            var firstCavalrymen = 11232;
            var experience = 4;
            var first = new ArmyComposition(firstSpearmen, firstArchers, firstCavalrymen, experience);

            var secondSpearmen = 43342;
            var secondArchers = 41233;
            var secondCavalrymen = 3134;
            var second = new ArmyComposition(secondSpearmen, secondArchers, secondCavalrymen);
            var result = ArmyComposition.Merge(first, second);
            
            Assert.True(result.Spearmen ==  firstSpearmen + secondSpearmen);
            Assert.True(result.Archers == firstArchers + secondArchers);
            Assert.True(result.Cavalrymen == firstCavalrymen + secondCavalrymen);
        }

        [Test]
        public void TestFight()
        {
            var spearmen = 44;
            var archers = 56;
            var cavalrymen = 65;
            var experience = 2;
           
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            spearmen = 43;
            archers = 4;
            cavalrymen = 31;
            var second = new ArmyComposition(spearmen, archers, cavalrymen);
            var result = ArmyComposition.Fight(first, second);
            
            Assert.True(result.ArmyPower() > 0);
        }

        [Test]
        public void TestSameArmiesWithDifferentExperience()
        {
            var spearmen = 44;
            var archers = 56;
            var cavalrymen = 65;
            var experience = 3;
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);

            experience = 2;
            var second = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            
            Assert.True(ArmyComposition.IsFirstWinner(first,second));
        }

        [Test]
        public void TestEqualArmiesFight()
        {
            var spearmen = 44;
            var archers = 56;
            var cavalrymen = 65;
            var experience = 2;
            var first = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            var second = new ArmyComposition(spearmen, archers, cavalrymen, experience);
            var result = ArmyComposition.Fight(first, second);

            var epsilon = 0.001; // If army power less than epsilon it is supposed to be empty
            Assert.True(Math.Abs(result.ArmyPower()) < epsilon);
        }
    }
}
