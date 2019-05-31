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
            var army = attackingArmy as UserArmy;
            army?.SetInactive();

            if (attackingArmy.PlayerType == PlayerType)
            {
                return Merge(attackingArmy, this);
            }
            return PerformBattle(attackingArmy, this);
        }

        public override Army CloneArmy()
        {
            var army = new UserArmy(PlayerType, ArmyComposition);

            if (isActive)
            {
                army.SetActive();
            }

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