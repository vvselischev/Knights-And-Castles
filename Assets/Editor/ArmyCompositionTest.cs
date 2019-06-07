using System;
using Assets.Scripts;
using NUnit.Framework;

namespace Editor
{
    public class ArmyCompositionTest
    {
        [Test]
        public void TestArmyPower()
        {
            var first = new ArmyComposition(3, 6 , 9);
            var second = new ArmyComposition(4, 8, 12);
            Assert.True(first.ArmyPower() < second.ArmyPower());
            
            first = new ArmyComposition(5, 1, 0, 2);
            second = new ArmyComposition(5, 1, 0, 3);
            Assert.True(first.ArmyPower() < second.ArmyPower());
        }

        [Test]
        public void TestIsFirstWinner()
        {
            var first = new ArmyComposition(6, 8, 10, 2);
            var second = new ArmyComposition(8, 18, 10, 2);
            var third = new ArmyComposition(8, 18, 10, 3);
            
            Assert.False(ArmyComposition.IsFirstWinner(first, second));
            Assert.True(ArmyComposition.IsFirstWinner(third, second));
        }

        [Test]
        public void TestTotalUnitQuality()
        {
            var first = new ArmyComposition(6, 8, 10, 2);
            var second = new ArmyComposition(8, 18, 10);

            Assert.True(first.TotalUnitQuantity() == 24);
            Assert.True(second.TotalUnitQuantity() == 36);
        }

        [Test]
        public void TestDeleteArmyPart()
        {
            var armyComposition = new ArmyComposition(45, 32, 12, 4);
            armyComposition.DeleteArmyPart(15, 22, 3);

            Assert.True(30 == armyComposition.Spearmen);
            Assert.True(10 == armyComposition.Archers);
            Assert.True(9 == armyComposition.Cavalrymen);
        }

        [Test]
        public void TestMerge()
        {
            var first = new ArmyComposition(32, 56, 65, 2);
            var second = new ArmyComposition(43, 4, 31);
            var result = ArmyComposition.Merge(first, second);
            
            Assert.True(result.Spearmen ==  75);
            Assert.True(result.Archers == 60);
            Assert.True(result.Cavalrymen == 96);
        }
        
        [Test]
        public void TestMergeBigArmies()
        {
            var first = new ArmyComposition(312, 5621, 6115, 2);
            var second = new ArmyComposition(4312, 467, 311);
            var result = ArmyComposition.Merge(first, second);
            
            Assert.True(result.Spearmen ==  4624);
            Assert.True(result.Archers == 6088);
            Assert.True(result.Cavalrymen == 6426);
        }

        [Test]
        public void TestFight()
        {
            var first = new ArmyComposition(44, 56, 65, 2);
            var second = new ArmyComposition(43, 4, 31);
            var result = ArmyComposition.Fight(first, second);
            
            Assert.True(result.ArmyPower() > 0);
        }

        [Test]
        public void TestSameArmiesWithDifferentExperience()
        {
            var first = new ArmyComposition(44, 56, 65, 3);
            var second = new ArmyComposition(44, 56, 65, 2);
            
            Assert.True(ArmyComposition.IsFirstWinner(first,second));
        }

        [Test]
        public void TestEqualArmiesFight()
        {
            var first = new ArmyComposition(44, 56, 65, 2);
            var second = new ArmyComposition(44, 56, 65, 2);
            var result = ArmyComposition.Fight(first, second);
            
            Assert.True(Math.Abs(result.ArmyPower()) < 0.01);
        }
    }
}
