using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum ArmyType
    {
        USER,
        NEUTRAL_FRIENDLY,
        NEUTRAL_AGRESSIVE
    }

    public abstract class Army
    {
        public readonly PlayerType playerType;
        public readonly ArmyType armyType;
        public ArmyComposition armyComposition;

        //TODO: fields about army composition, methods Merge, Battle etc.

        public Army(ArmyType armyType, PlayerType playerType, ArmyComposition armyComposition)
        {
            this.armyType = armyType;
            this.playerType = playerType;
            this.armyComposition = armyComposition;
            if (!CheckValidTypes())
            {
                throw new Exception("In Army constructor incompatible types were received");
            }
        }

        private bool CheckValidTypes()
        {
            return (((armyType == ArmyType.NEUTRAL_AGRESSIVE) || (armyType == ArmyType.NEUTRAL_FRIENDLY)) &&
                    (playerType == PlayerType.NEUTRAL)) || ((armyType == ArmyType.USER) &&
                                                            ((playerType == PlayerType.FIRST) ||
                                                             playerType == PlayerType.SECOND));
        }

        public abstract Army PerformAction(Army attackingArmy);

        public static Army Merge(Army firstArmy, Army secondArmy)
        {
            if (firstArmy.armyType == ArmyType.USER && secondArmy.armyType == ArmyType.USER)
            {
                return MergeUserArmies(firstArmy, secondArmy);
            }
            if (firstArmy.armyType == ArmyType.USER && secondArmy.playerType == PlayerType.NEUTRAL)
            {
                return MergeUserAndNeutralArmy(firstArmy, secondArmy);
            }
            if (firstArmy.playerType == PlayerType.NEUTRAL && secondArmy.armyType == ArmyType.USER)
            {
                return MergeUserAndNeutralArmy(secondArmy, firstArmy);
            }
            throw new Exception("Merge: armies cannot be merged");
        }

        private static UserArmy MergeUserArmies(Army firstArmy, Army secondArmy)
        {
            if (firstArmy.playerType == secondArmy.playerType)
            {
                return new UserArmy(firstArmy.playerType,
                    ArmyComposition.Merge(firstArmy.armyComposition, secondArmy.armyComposition));
            }
            else
            {
                throw new Exception("MergeUserArmies: players types are different");
            }
        }

        private static UserArmy MergeUserAndNeutralArmy(Army userArmy, Army neutralArmy)
        {
            if (neutralArmy.armyType == ArmyType.NEUTRAL_FRIENDLY)
            {
                return new UserArmy(userArmy.playerType,
                    ArmyComposition.Merge(userArmy.armyComposition, neutralArmy.armyComposition));
            }
            throw new Exception("MergeUserAndNeutralArmies: neutral army type is not NEUTRAL_FRIENDLY");
        }

        public static Army PerformBattle(Army firstArmy, Army secondArmy)
        {
            if (ArmyComposition.IsFirstWinner(firstArmy.armyComposition, secondArmy.armyComposition))
            {
                return CalcResultArmies(firstArmy, secondArmy);
            }
            return CalcResultArmies(secondArmy, firstArmy);
        }

        private static Army CalcResultArmies(Army winnerArmy, Army loserArmy)
        {
            ArmyComposition resultArmyComposition = ArmyComposition.Fight(
                winnerArmy.armyComposition, loserArmy.armyComposition);
            if (winnerArmy.armyType == ArmyType.USER)
            {
                return new UserArmy(winnerArmy.playerType, resultArmyComposition);
            }
            if (winnerArmy.armyType == ArmyType.NEUTRAL_AGRESSIVE)
            {
                return new NeutralAggressiveArmy(resultArmyComposition);
            }
            throw new Exception("PerformBattle: Inappropriate army types");
        }

        public Army Split(int spearmen, int archers, int cavalrymen)
        {
            armyComposition.DeleteArmyPart(spearmen, archers, cavalrymen);

            if (armyType == ArmyType.USER)
            {
                return new UserArmy(playerType, new ArmyComposition(spearmen,
                    archers, cavalrymen, armyComposition.experience));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
