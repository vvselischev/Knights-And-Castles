using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of army composition a game
    /// </summary>
    public class ArmyComposition
    {
        /// <summary>
        /// Number of spearmen in army
        /// </summary>
        public int Spearmen { get; private set; }
        
        /// <summary>
        /// Number of archers in army
        /// </summary>
        public int Archers { get; private set; }
        
        /// <summary>
        /// Number of cavalrymen in army
        /// </summary>
        public int Cavalrymen { get; private set; }
        
        /// <summary>
        /// Army experience
        /// </summary>
        public double Experience { get; private set; }

        public ArmyComposition(int spearmen, int archers, int cavalrymen, double experience = 1)
        {
            Spearmen = spearmen;
            Archers = archers;
            Cavalrymen = cavalrymen;
            Experience = experience;
        }

        /// <summary>
        /// Converts army composition to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Spearmen: " + Spearmen + "\n" + "Archers:    " + Archers + "\n" + "Cavalry:    " + Cavalrymen + "\n" +
                   "Experience: " + Math.Round(Experience, 2);
        }

        /// <summary>
        /// Merges two army compositions.
        /// The old experience is uniformly distributed among the total number of units in each army.
        /// </summary>
        public static ArmyComposition Merge(ArmyComposition first, ArmyComposition second)
        {
            var newExperience = (first.TotalUnitQuantity() * first.Experience +
                                   second.TotalUnitQuantity() * second.Experience) /
                                   (first.TotalUnitQuantity() + second.TotalUnitQuantity());

            return new ArmyComposition(first.Spearmen + second.Spearmen,
                                       first.Archers + second.Archers,
                                       first.Cavalrymen + second.Cavalrymen,
                                       newExperience);
        }

        /// <summary>
        /// Performs a fight between two army compositions.
        /// Updates the experience of the winner army.
        /// </summary>
        /// <param name="winnerArmyComposition"></param>
        /// <param name="loserArmyComposition"></param>
        /// <returns></returns>
        public static ArmyComposition Fight(ArmyComposition winnerArmyComposition, 
                                            ArmyComposition loserArmyComposition)
        {
            var powerDifference = winnerArmyComposition.ArmyPower() - loserArmyComposition.ArmyPower();
            
            //The losses have the square root dependency.
            var mortalityRate = Math.Sqrt(powerDifference / winnerArmyComposition.ArmyPower());
            
            //The experience increased quadratically, based on the relative power of armies.
            //Quadratic dependency mainly to decrease the huge role of the experience at the final part of the game.
            var experienceIncrease = 1 + Math.Pow(loserArmyComposition.ArmyPower() / winnerArmyComposition.ArmyPower(), 2);
            return winnerArmyComposition.ArmyCompositionAfterFight(mortalityRate, 
                            winnerArmyComposition.Experience * experienceIncrease);
        }

        /// <summary>
        /// Returns a new army composition based on given units alive part and new experience.
        /// </summary>
        /// <param name="mortalityRate"> The relative part of units that remain alive. </param>
        /// <param name="experience"> The new experience. </param>
        private ArmyComposition ArmyCompositionAfterFight(double mortalityRate, double experience)
        {
            return new ArmyComposition((int)Math.Ceiling(Spearmen * mortalityRate),
                (int)Math.Ceiling(Archers * mortalityRate), (int)Math.Ceiling(Cavalrymen * mortalityRate), experience);
        }

        /// <summary>
        /// Checks that first army is more powerful than second.
        /// </summary>
        public static bool IsFirstWinner(ArmyComposition firstArmyComposition,
                                          ArmyComposition secondArmyComposition)
        {
            var firstArmyPower = firstArmyComposition.ArmyPower();
            var secondArmyPower = secondArmyComposition.ArmyPower();
            return firstArmyPower >= secondArmyPower;
        }

        /// <summary>
        /// Calculates total number of units in the army composition.
        /// </summary>
        /// <returns></returns>
        public int TotalUnitQuantity()
        {
            return Spearmen + Archers + Cavalrymen;
        }

        /// <summary>
        /// Calculates the army power as the product of total number of units and the experience.
        /// </summary>
        /// <returns></returns>
        public double ArmyPower()
        {
            return TotalUnitQuantity() * Experience;
        }

        /// <summary>
        /// Removes the given army part.
        /// If the number of units to remove is greater than in this army, then zero units of that type remain.
        /// </summary>
        public void DeleteArmyPart(int spearmen, int archers, int cavalrymen)
        {
            Spearmen = Math.Max(0, Spearmen - spearmen);
            Archers = Math.Max(0, Archers - archers);
            Cavalrymen = Math.Max(0, Cavalrymen - cavalrymen);
        }
    }
}
