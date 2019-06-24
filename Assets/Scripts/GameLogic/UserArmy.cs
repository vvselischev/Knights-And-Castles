namespace Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of a user army in a game.
    /// Army can be active or not.
    /// </summary>
    public class UserArmy : Army
    {
        /// <summary>
        /// True if army is active.
        /// </summary>
        private bool isActive;

        public UserArmy(PlayerType playerType, ArmyComposition armyComposition) : 
            base(ArmyType.USER, playerType, armyComposition)
        {
            SetInactive();
        }

        /// <summary>
        /// Merges or fights depends on type of attacking army.
        /// If armies' player types are equal, then merges, otherwise performs a fight
        /// and returns the winner army.
        /// Both armies become inactive after this action.
        /// </summary>
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
        /// Splits a given part from army with given composition.
        /// Both armies become inactive after this action.
        /// </summary>
        protected override Army Split(int spearmen, int archers, int cavalrymen)
        {
            ArmyComposition.DeleteArmyPart(spearmen, archers, cavalrymen);
            SetInactive();
            //New army is inactive by default.
            return new UserArmy(PlayerType, new ArmyComposition(spearmen, archers, cavalrymen, ArmyComposition.Experience));
        }

        /// <summary>
        /// Creates new user army with same army composition
        /// </summary>
        public override Army CloneArmy()
        {
            var army = new UserArmy(PlayerType, ArmyComposition);

            //Because by default armies are not active.
            if (isActive)
            {
                army.SetActive();
            }
            return army;
        }

        /// <summary>
        /// Checks that army is active.
        /// </summary>
        public bool IsActive()
        {
            return isActive;
        }

        /// <summary>
        /// Activate the army.
        /// </summary>
        public override void SetActive()
        {
            isActive = true;
        }

        /// <summary>
        /// Deactivates the army.
        /// </summary>
        public void SetInactive()
        {
            isActive = false;
        }
    }
}