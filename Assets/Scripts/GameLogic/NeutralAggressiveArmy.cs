namespace Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of neutral aggressive army in game
    /// </summary>
    public class NeutralAggressiveArmy : Army
    {
        public NeutralAggressiveArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_AGGRESSIVE, PlayerType.NEUTRAL, armyComposition)
        {
        }

        /// <summary>
        /// This army always fights with other armies
        /// </summary>
        /// <param name="attackingArmy"> Army to fight with </param>
        /// <returns> Result of fighting </returns>
        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }

        /// <summary>
        /// Creates new army with same composition
        /// </summary>
        /// <returns></returns>
        public override Army CloneArmy()
        {
            return new NeutralAggressiveArmy(ArmyComposition);
        }
    }
}