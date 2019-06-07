using UnityEngine;

namespace Assets.Scripts
{
    public class Castle : BoardStorageItem
    {
        private PlayerType ownerType;
        private PlayGameState playGameState;

        public bool PerformAction(Army enteredArmy)
        {
            if (enteredArmy.PlayerType != ownerType)
            {
                if (enteredArmy.PlayerType == PlayerType.FIRST)
                {
                    playGameState.OnFinishGame(ResultType.FIRST_WIN);    
                }
                else if (enteredArmy.PlayerType == PlayerType.SECOND)
                {
                    playGameState.OnFinishGame(ResultType.SECOND_WIN);
                }
                return true;
            }
            return false;
        }

        public Castle(GameObject targetObject, PlayerType ownerType) : base(targetObject)
        {
            this.ownerType = ownerType;
            //TODO: awful solution...
            playGameState = StateManager.Instance.CurrentState as PlayGameState;
        }
    }
}