using UnityEngine;

namespace Assets.Scripts
{
    public class ChooseModeButton : MonoBehaviour
    {
        [SerializeField] private StateType nextState;

        public void OnClick()
        {
            var stateManager = StateManager.Instance;         
            
            //We do not want several board modes in network in thi version.
            if (nextState != StateType.LOBBY_GAME_STATE)
            {
                (stateManager.GetState(StateType.CHOOSE_BOARD_GAME_STATE) as ChooseBoardGameState).NextStateType = nextState;
                stateManager.ChangeState(StateType.CHOOSE_BOARD_GAME_STATE);
            }
            else
            {
                stateManager.ChangeState(StateType.LOBBY_GAME_STATE);
            }
        }
    }
}