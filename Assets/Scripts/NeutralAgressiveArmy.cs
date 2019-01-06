using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class NeutralAgressiveArmy : Army
    {
        public NeutralAgressiveArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_AGRESSIVE, PlayerType.NEUTRAL, armyComposition)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }
    }
}
