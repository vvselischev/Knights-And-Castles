using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class ArmyComposition
    {
        public int mice;
        public int cats;
        public int elephants;

        public ArmyComposition(int mice, int cats, int elephants)
        {
            this.mice = mice;
            this.cats = cats;
            this.elephants = elephants;
        }

        public static ArmyComposition Merge(ArmyComposition first, ArmyComposition second)
        {
            return new ArmyComposition(first.mice + second.mice,
                                       first.cats + second.cats,
                                       first.elephants + second.elephants);
        }

        public void Nullify()
        {
            this.mice = 0;
            this.cats = 0;
            this.elephants = 0;
        }

        public static ArmyComposition CalcResultArmiesComposition(
           ArmyComposition winnerArmyComposition, ArmyComposition loserArmyComposition)
        {
            loserArmyComposition.Nullify();
            return winnerArmyComposition;
        }

        public static bool IsFirstWinner(ArmyComposition firstArmyComposition,
                                          ArmyComposition secondArmyComposition)
        {
            int firstArmyPower = firstArmyComposition.FindArmyCompositionPower();
            int secondArmyPower = secondArmyComposition.FindArmyCompositionPower();
            return firstArmyPower >= secondArmyPower;
        }

        public int FindArmyCompositionPower()
        {
            return mice + cats + elephants;
        }
    }
}
