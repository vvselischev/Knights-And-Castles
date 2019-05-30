using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseModeButton : MonoBehaviour
    {
        public StateType nextState;

        public void OnClick()
        {
            var stateManager = StateManager.Instance;
            (stateManager.states[StateType.CHOOSE_BOARD_GAME_STATE] as ChooseBoardGameState).NextStateType = nextState;
            
            //We do not want several board modes in network at this time.
            if (nextState != StateType.NETWORK_GAME_STATE)
            {
                stateManager.ChangeState(StateType.CHOOSE_BOARD_GAME_STATE);
            }
            else
            {
                stateManager.ChangeState(StateType.LOBBY_GAME_STATE);
            }
        }
    }
}