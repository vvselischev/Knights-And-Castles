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

    public class Army
    {
        public readonly PlayerType playerType;
        public readonly ArmyType armyType;

        //TODO: fields about army composition, methods Merge, Battle etc.

        public Army(ArmyType armyType, PlayerType playerType)
        {
            this.armyType = armyType;
            this.playerType = playerType;
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
    }
}
