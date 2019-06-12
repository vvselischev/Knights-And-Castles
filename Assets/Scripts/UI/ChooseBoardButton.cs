using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Button of choosing board mode.
    /// Initializes remembered in ChooseBoard game state play mode and switches the game to it.
    /// </summary>
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
                stateManager.networkPlayGameState.ConfigurationType = boardType;
            }
            else
            {
                (stateManager.GetState(nextStateType) as PlayGameState).ConfigurationType = boardType;
            }

            stateManager.ChangeState(nextStateType);
        }
    }
}