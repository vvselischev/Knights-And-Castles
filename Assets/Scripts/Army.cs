﻿using System;

 namespace Assets.Scripts
{
    public enum ArmyType
    {
        USER,
        NEUTRAL_FRIENDLY,
        NEUTRAL_AGGRESSIVE
    }

    public abstract class Army
    {
        public PlayerType PlayerType { get; }
        private ArmyType armyType;
        public ArmyComposition ArmyComposition { get; }
        
        protected Army(ArmyType armyType, PlayerType playerType, ArmyComposition armyComposition)
        {
            this.armyType = armyType;
            PlayerType = playerType;
            ArmyComposition = armyComposition;
            if (!CheckValidTypes())
            {
                throw new Exception("In Army constructor incompatible types were received");
            }
        }

        public virtual void SetActive() {}

        private bool CheckValidTypes()
        {
            return (((armyType == ArmyType.NEUTRAL_AGGRESSIVE) || (armyType == ArmyType.NEUTRAL_FRIENDLY)) &&
                    (PlayerType == PlayerType.NEUTRAL)) || ((armyType == ArmyType.USER) &&
                                                            ((PlayerType == PlayerType.FIRST) ||
                                                             PlayerType == PlayerType.SECOND));
        }

        public abstract Army PerformAction(Army attackingArmy);

        public static Army Merge(Army firstArmy, Army secondArmy)
        {
            if (firstArmy.armyType == ArmyType.USER && secondArmy.armyType == ArmyType.USER)
            {
                return MergeUserArmies(firstArmy, secondArmy);
            }
            if (firstArmy.armyType == ArmyType.USER && secondArmy.PlayerType == PlayerType.NEUTRAL)
            {
                return MergeUserAndNeutralArmy(firstArmy, secondArmy);
            }
            if (firstArmy.PlayerType == PlayerType.NEUTRAL && secondArmy.armyType == ArmyType.USER)
            {
                return MergeUserAndNeutralArmy(secondArmy, firstArmy);
            }
            throw new Exception("Merge: armies cannot be merged");
        }

        private static UserArmy MergeUserArmies(Army firstArmy, Army secondArmy)
        {
            if (firstArmy.PlayerType == secondArmy.PlayerType)
            {
                return new UserArmy(firstArmy.PlayerType,
                    ArmyComposition.Merge(firstArmy.ArmyComposition, secondArmy.ArmyComposition));
            }

            throw new Exception("MergeUserArmies: players types are different");
        }

        private static UserArmy MergeUserAndNeutralArmy(Army userArmy, Army neutralArmy)
        {
            if (neutralArmy.armyType == ArmyType.NEUTRAL_FRIENDLY)
            {
                return new UserArmy(userArmy.PlayerType,
                    ArmyComposition.Merge(userArmy.ArmyComposition, neutralArmy.ArmyComposition));
            }
            throw new Exception("MergeUserAndNeutralArmies: neutral army type is not NEUTRAL_FRIENDLY");
        }

        protected static Army PerformBattle(Army firstArmy, Army secondArmy)
        {
            if (ArmyComposition.IsFirstWinner(firstArmy.ArmyComposition, secondArmy.ArmyComposition))
            {
                return CalcResultArmies(firstArmy, secondArmy);
            }
            return CalcResultArmies(secondArmy, firstArmy);
        }

        private static Army CalcResultArmies(Army winnerArmy, Army loserArmy)
        {
            var resultArmyComposition = ArmyComposition.Fight(
                winnerArmy.ArmyComposition, loserArmy.ArmyComposition);
            if (winnerArmy.armyType == ArmyType.USER)
            {
                return new UserArmy(winnerArmy.PlayerType, resultArmyComposition);
            }
            if (winnerArmy.armyType == ArmyType.NEUTRAL_AGGRESSIVE)
            {
                return new NeutralAggressiveArmy(resultArmyComposition);
            }
            throw new Exception("PerformBattle: Inappropriate army types");
        }

        protected virtual Army Split(int spearmen, int archers, int cavalrymen)
        {
            ArmyComposition.DeleteArmyPart(spearmen, archers, cavalrymen);
            return this;
        }

        public double ArmyPower()
        {
            return ArmyComposition.ArmyPower();
        }

        public Army SplitIntoEqualParts() {
            var spearmen = ArmyComposition.Spearmen / 2;
            var archers = ArmyComposition.Archers / 2;
            var cavalrymen = ArmyComposition.Cavalrymen / 2;

            return Split(spearmen, archers, cavalrymen);
        }

        public abstract Army CloneArmy();
    }
}