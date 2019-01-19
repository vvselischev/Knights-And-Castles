using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ArmyComposition
    {
        public int spearmen;
        public int archers;
        public int cavalrymen;
        public double experience;

        public ArmyComposition(int spearmen, int archers, int cavalrymen, double experience = 1)
        {
            this.spearmen = spearmen;
            this.archers = archers;
            this.cavalrymen = cavalrymen;
            this.experience = experience;
        }

        public override string ToString()
        {
            return "s:" + spearmen.ToString() + " a:" + archers.ToString() + " c:" + cavalrymen.ToString() +
                   " e:" + Math.Round(experience, 2).ToString();
        }

        public static ArmyComposition Merge(ArmyComposition first, ArmyComposition second)
        {
            double newExperience = (first.TotalUnitQuantity() * first.experience +
                                   second.TotalUnitQuantity() * second.experience) /
                                   (first.TotalUnitQuantity() + second.TotalUnitQuantity());

            return new ArmyComposition(first.spearmen + second.spearmen,
                                       first.archers + second.archers,
                                       first.cavalrymen + second.cavalrymen,
                                       newExperience);
        }

        public void Nullify()
        {
            this.spearmen = 0;
            this.archers = 0;
            this.cavalrymen = 0;
        }

        public static ArmyComposition Fight(ArmyComposition winnerArmyComposition, 
                                            ArmyComposition loserArmyComposition)
        {
            double powerDifference = winnerArmyComposition.ArmyPower() - loserArmyComposition.ArmyPower();
            double mortalityRate = Math.Sqrt(powerDifference / winnerArmyComposition.ArmyPower());
            double experienceIncrease = 1 + loserArmyComposition.ArmyPower() / winnerArmyComposition.ArmyPower();
            return winnerArmyComposition.ArmyCompositionAfterFight(mortalityRate, 
                            winnerArmyComposition.experience * experienceIncrease);
        }

        private ArmyComposition ArmyCompositionAfterFight(double mortalityRate, double experience)
        {
            return new ArmyComposition((int)Math.Ceiling(spearmen * mortalityRate),
                (int)Math.Ceiling(archers * mortalityRate), (int)Math.Ceiling(cavalrymen * mortalityRate), experience);
        }

        public static bool IsFirstWinner(ArmyComposition firstArmyComposition,
                                          ArmyComposition secondArmyComposition)
        {
            double firstArmyPower = firstArmyComposition.ArmyPower();
            double secondArmyPower = secondArmyComposition.ArmyPower();
            return firstArmyPower >= secondArmyPower;
        }

        public int TotalUnitQuantity()
        {
            return spearmen + archers + cavalrymen;
        }

        public double ArmyPower()
        {
            return TotalUnitQuantity() * experience;
        }
    }
}
