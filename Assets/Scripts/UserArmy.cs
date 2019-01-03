using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class UserArmy : Army
    {
        public UserArmy(PlayerType playerType) : base(ArmyType.USER, playerType)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }
    }
}
