namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of user army in a game
    /// </summary>
    public class UserArmy : Army
    {
        /// <summary>
        /// True if army is active
        /// </summary>
        private bool isActive;

        public UserArmy(PlayerType playerType, ArmyComposition armyComposition) : 
            base(ArmyType.USER, playerType, armyComposition)
        {
            isActive = false;
        }

        /// <summary>
        /// Merges or fights depends on type of attacking army 
        /// </summary>
        /// <param name="attackingArmy"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Splits a part from army with given composition
        /// </summary>
        /// <param name="spearmen"></param>
        /// <param name="archers"></param>
        /// <param name="cavalrymen"></param>
        /// <returns></returns>
        protected override Army Split(int spearmen, int archers, int cavalrymen)
        {
            ArmyComposition.DeleteArmyPart(spearmen, archers, cavalrymen);
            SetInactive();
            return new UserArmy(PlayerType, new ArmyComposition(spearmen, archers, cavalrymen, ArmyComposition.Experience));
        }

        /// <summary>
        /// Creates new user army with same army composition
        /// </summary>
        /// <returns></returns>
        public override Army CloneArmy()
        {
            var army = new UserArmy(PlayerType, ArmyComposition);

            if (isActive)
            {
                army.SetActive();
            }

            return army;
        }

        /// <summary>
        /// Checks that army is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return isActive;
        }

        /// <summary>
        /// Activate army
        /// </summary>
        public override void SetActive()
        {
            isActive = true;
        }

        /// <summary>
        /// Deactivate army
        /// </summary>
        public void SetInactive()
        {
            isActive = false;
        }
    }
}