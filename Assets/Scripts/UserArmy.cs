using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class UserArmy : Army
    {
        private bool isActive;

        public UserArmy(PlayerType playerType, ArmyComposition armyComposition) : 
            base(ArmyType.USER, playerType, armyComposition)
        {
            isActive = false;
        }

        public override Army PerformAction(Army attackingArmy)
        {
            setInactive();
            if (attackingArmy is UserArmy)
            {
                (attackingArmy as UserArmy).setInactive();
            }

            if (attackingArmy.playerType == playerType)
            {
                return Merge(attackingArmy, this);
            }
            else
            {
                return PerformBattle(attackingArmy, this);
            }
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void setActive()
        {
            isActive = true;
        }

        public void setInactive()
        {
            isActive = false;
        }
    }
}
