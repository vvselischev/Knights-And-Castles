using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseBoardButton : MonoBehaviour
    {
        [SerializeField] private ChooseBoardGameState chooseBoardGameState;
        [SerializeField] private BoardType boardType;

        public void OnClick()
        {
            var stateManager = StateManager.Instance;
            var nextStateType = chooseBoardGameState.NextStateType;
            (stateManager.GetState(nextStateType) as PlayGameState).ConfigurationType = boardType;
            stateManager.ChangeState(nextStateType);
        }
    }
}