using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class NeutralAgressiveArmy : Army
    {
        public NeutralAgressiveArmy() : base(ArmyType.NEUTRAL_AGRESSIVE, PlayerType.NEUTRAL)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }
    }
}
