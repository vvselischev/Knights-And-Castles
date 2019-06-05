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
            if (nextStateType == StateType.LOBBY_GAME_STATE)
            {
                (stateManager.GetState(StateType.LOBBY_GAME_STATE) as LobbyGameState).ConfigurationType = boardType;
            }
            else
            {
                (stateManager.GetState(nextStateType) as PlayGameState).ConfigurationType = boardType;
            }

            stateManager.ChangeState(nextStateType);
        }
    }
}