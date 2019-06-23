﻿using System;

 namespace Assets.Scripts
{
    /// <summary>
    /// Stores type of army
    /// </summary>
    public enum ArmyType
    {
        /// <summary>
        /// Armies of this type are controlled by users.
        /// </summary>
        USER,
        
        /// <summary>
        /// Armies of this type join to user army.
        /// </summary>
        NEUTRAL_FRIENDLY,
        
        /// <summary>
        /// Armies of this type fight with user armies.
        /// </summary>
        NEUTRAL_AGGRESSIVE
    }

    /// <summary>
    /// Implementation of army in a game
    /// </summary>
    public abstract class Army
    {
        /// <summary>
        /// Stores owner type
        /// </summary>
        public PlayerType PlayerType { get; }
        
        /// <summary>
        /// Stores type of army
        /// </summary>
        private ArmyType armyType;
        
        /// <summary>
        /// Stores composition of army
        /// </summary>
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

        /// <summary>
        /// Activates army
        /// </summary>
        public virtual void SetActive() {}

        private bool CheckValidTypes()
        {
            return (((armyType == ArmyType.NEUTRAL_AGGRESSIVE) || (armyType == ArmyType.NEUTRAL_FRIENDLY)) &&
                    (PlayerType == PlayerType.NEUTRAL)) || ((armyType == ArmyType.USER) &&
                                                            ((PlayerType == PlayerType.FIRST) ||
                                                             PlayerType == PlayerType.SECOND));
        }

        /// <summary>
        /// Performs fight or merge depends on army type
        /// </summary>
        /// <param name="attackingArmy"></param>
        /// <returns></returns>
        public abstract Army PerformAction(Army attackingArmy);

        /// <summary>
        /// Merges two given armies.
        /// Returns a new one that has the number of particular units equal to the sum
        /// of the corresponding numbers of the arguments.
        /// If two user armies are merged, they must have the same PlayerType.
        /// If one army is NeutralFriendly, then another one must be User's.
        /// In all other cases exceptions are thrown.
        /// </summary>
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

        /// <summary>
        /// Checks that both armies have the same owner and merges them to the new one.
        /// Otherwise, an exception is thrown.
        /// </summary>
        private static UserArmy MergeUserArmies(Army firstArmy, Army secondArmy)
        {
            if (firstArmy.PlayerType == secondArmy.PlayerType)
            {
                return new UserArmy(firstArmy.PlayerType,
                    ArmyComposition.Merge(firstArmy.ArmyComposition, secondArmy.ArmyComposition));
            }

            throw new Exception("MergeUserArmies: players types are different");
        }
        
        /// <summary>
        /// Checks that the second army is neutral friendly and merges them to the new one.
        /// Otherwise, an exception is thrown.
        /// </summary>
        private static UserArmy MergeUserAndNeutralArmy(Army userArmy, Army neutralArmy)
        {
            if (neutralArmy.armyType == ArmyType.NEUTRAL_FRIENDLY)
            {
                return new UserArmy(userArmy.PlayerType,
                    ArmyComposition.Merge(userArmy.ArmyComposition, neutralArmy.ArmyComposition));
            }
            throw new Exception("MergeUserAndNeutralArmies: neutral army type is not NEUTRAL_FRIENDLY");
        }

        /// <summary>
        /// Simulates a fight between two given armies.
        /// Returns an army of the winner with losses.
        /// </summary>
        protected static Army PerformBattle(Army firstArmy, Army secondArmy)
        {
            if (ArmyComposition.IsFirstWinner(firstArmy.ArmyComposition, secondArmy.ArmyComposition))
            {
                return CalcResultArmies(firstArmy, secondArmy);
            }
            return CalcResultArmies(secondArmy, firstArmy);
        }

        /// <summary>
        /// Given the winner and loser armies, returns a new army of the winner of the corresponding type with losses.
        /// </summary>
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

        /// <summary>
        /// Splits the army.
        /// Default behaviour: removes the given number of units of the current army and returns this army.
        /// </summary>
        protected virtual Army Split(int spearmen, int archers, int cavalrymen)
        {
            ArmyComposition.DeleteArmyPart(spearmen, archers, cavalrymen);
            return this;
        }

        /// <summary>
        /// Calculates army power.
        /// </summary>
        /// <returns></returns>
        public double ArmyPower()
        {
            return ArmyComposition.ArmyPower();
        }

        /// <summary>
        /// Splits army into two equal parts.
        /// </summary>
        /// <returns> Another part. </returns>
        public Army SplitIntoEqualParts() {
            var spearmen = ArmyComposition.Spearmen / 2;
            var archers = ArmyComposition.Archers / 2;
            var cavalrymen = ArmyComposition.Cavalrymen / 2;

            return Split(spearmen, archers, cavalrymen);
        }

        /// <summary>
        /// Clones army.
        /// </summary>
        /// <returns></returns>
        public abstract Army CloneArmy();
    }
}