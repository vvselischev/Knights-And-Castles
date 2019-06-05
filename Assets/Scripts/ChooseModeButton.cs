using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseModeButton : MonoBehaviour
    {
        [SerializeField] private StateType nextState;

        public void OnClick()
        {
            var stateManager = StateManager.Instance;

            (stateManager.GetState(StateType.CHOOSE_BOARD_GAME_STATE) as ChooseBoardGameState).NextStateType =
                nextState;
            stateManager.ChangeState(StateType.CHOOSE_BOARD_GAME_STATE);
        }
    }
}