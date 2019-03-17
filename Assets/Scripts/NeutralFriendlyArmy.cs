using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class NeutralFriendlyArmy : Army
    {
        public NeutralFriendlyArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_FRIENDLY, PlayerType.NEUTRAL, armyComposition)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return Merge(this, attackingArmy);
        }

        public override object Clone()
        {
            return new NeutralFriendlyArmy(armyComposition);
        }
    }
}
