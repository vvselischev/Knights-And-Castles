namespace Assets.Scripts
{
    public class NeutralAggressiveArmy : Army
    {
        public NeutralAggressiveArmy(ArmyComposition armyComposition) : 
            base(ArmyType.NEUTRAL_AGGRESSIVE, PlayerType.NEUTRAL, armyComposition)
        {
        }

        public override Army PerformAction(Army attackingArmy)
        {
            return PerformBattle(attackingArmy, this);
        }

        public override Army CloneArmy()
        {
            return new NeutralAggressiveArmy(ArmyComposition);
        }
    }
}