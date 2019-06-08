namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of neutral friendly army in game
    /// </summary>
    public class NeutralFriendlyArmy : Army
    {
        public NeutralFriendlyArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_FRIENDLY, PlayerType.NEUTRAL, armyComposition)
        {
        }

        /// <summary>
        /// This army merges with all other armies
        /// </summary>
        /// <param name="attackingArmy"> Army to merge with </param>
        /// <returns> New army, which is a result of merge </returns>
        public override Army PerformAction(Army attackingArmy)
        {
            return Merge(this, attackingArmy);
        }

        /// <summary>
        /// Creates new army with same composition
        /// </summary>
        /// <returns></returns>
        public override Army CloneArmy()
        {
            return new NeutralFriendlyArmy(ArmyComposition);
        }
    }
}