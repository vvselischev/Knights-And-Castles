using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseBoardButton : MonoBehaviour
    {
        public ChooseBoardGameState chooseBoardGameState;
        public BoardType boardType;

        public void OnClick()
        {
            var stateManager = StateManager.Instance;
            var nextStateType = chooseBoardGameState.NextStateType;
            (stateManager.states[nextStateType] as PlayGameState).configurationType = boardType;
            stateManager.ChangeState(nextStateType);
        }
    }
}