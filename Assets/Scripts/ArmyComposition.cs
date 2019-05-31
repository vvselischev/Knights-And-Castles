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
        public int Spearmen { get; private set; }
        public int Archers { get; private set; }
        public int Cavalrymen { get; private set; }
        public double Experience { get; private set; }

        public ArmyComposition(int spearmen, int archers, int cavalrymen, double experience = 1)
        {
            Spearmen = spearmen;
            Archers = archers;
            Cavalrymen = cavalrymen;
            Experience = experience;
        }

        public override string ToString()
        {
            return "Spearmen: " + Spearmen + "\n" + "Archers:    " + Archers + "\n" + "Cavalry:    " + Cavalrymen + "\n" +
                   "Experience: " + Math.Round(Experience, 2);
        }

        public static ArmyComposition Merge(ArmyComposition first, ArmyComposition second)
        {
            double newExperience = (first.TotalUnitQuantity() * first.Experience +
                                   second.TotalUnitQuantity() * second.Experience) /
                                   (first.TotalUnitQuantity() + second.TotalUnitQuantity());

            return new ArmyComposition(first.Spearmen + second.Spearmen,
                                       first.Archers + second.Archers,
                                       first.Cavalrymen + second.Cavalrymen,
                                       newExperience);
        }

        public static ArmyComposition Fight(ArmyComposition winnerArmyComposition, 
                                            ArmyComposition loserArmyComposition)
        {
            double powerDifference = winnerArmyComposition.ArmyPower() - loserArmyComposition.ArmyPower();
            double mortalityRate = Math.Sqrt(powerDifference / winnerArmyComposition.ArmyPower());
            double experienceIncrease = 1 + loserArmyComposition.ArmyPower() / winnerArmyComposition.ArmyPower();
            return winnerArmyComposition.ArmyCompositionAfterFight(mortalityRate, 
                            winnerArmyComposition.Experience * experienceIncrease);
        }

        private ArmyComposition ArmyCompositionAfterFight(double mortalityRate, double experience)
        {
            return new ArmyComposition((int)Math.Ceiling(Spearmen * mortalityRate),
                (int)Math.Ceiling(Archers * mortalityRate), (int)Math.Ceiling(Cavalrymen * mortalityRate), experience);
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
            return Spearmen + Archers + Cavalrymen;
        }

        public double ArmyPower()
        {
            return TotalUnitQuantity() * Experience;
        }

        public void DeleteArmyPart(int spearmen, int archers, int cavalrymen)
        {
            Spearmen = Math.Max(0, Spearmen - spearmen);
            Archers = Math.Max(0, Archers - archers);
            Cavalrymen = Math.Max(0, Cavalrymen - cavalrymen);
        }
    }
}
