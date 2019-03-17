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
            SetInactive();
            if (attackingArmy is UserArmy)
            {
                (attackingArmy as UserArmy).SetInactive();
            }

            if (attackingArmy.playerType == playerType)
            {
                return Merge(attackingArmy, this);
            }
            return PerformBattle(attackingArmy, this);
        }

        public override object Clone()
        {
            var army = new UserArmy(playerType, armyComposition);

            army.SetActive();
            return army;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public override void SetActive()
        {
            isActive = true;
        }

        public void SetInactive()
        {
            isActive = false;
        }
    }
}
