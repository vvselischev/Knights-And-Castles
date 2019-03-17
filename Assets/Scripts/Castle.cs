using UnityEngine;

namespace Assets.Scripts
{
    public class Castle : BoardStorageItem
    {
        public PlayerType ownerType;
        public PlayGameState playGameState;

        public bool PerformAction(Army enteredArmy)
        {
            Debug.Log("Entered: " + enteredArmy.playerType + "; Owner: " + ownerType);
            if (enteredArmy.playerType != ownerType)
            {
                if (enteredArmy.playerType == PlayerType.FIRST)
                {
                    playGameState.OnFinishGame(ResultType.FIRST_WIN);    
                }
                else if (enteredArmy.playerType == PlayerType.SECOND)
                {
                    playGameState.OnFinishGame(ResultType.SECOND_WIN);
                }
                return true;
            }
            return false;
        }

        public Castle(GameObject targetObject) : base(targetObject)
        {
        }
    }
}