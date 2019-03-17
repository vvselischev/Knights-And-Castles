using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class NeutralAggressiveArmy : Army
    {
        public NeutralAggressiveArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_AGGRESSIVE, PlayerType.NEUTRAL, armyComposition)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }

        public override object Clone()
        {
            return new NeutralAggressiveArmy(armyComposition);
        }
    }
}
