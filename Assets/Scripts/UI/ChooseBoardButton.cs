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
        /// <summary>
        /// Board type to initialize a game state.
        /// </summary>
        [SerializeField] private BoardType boardType;

        /// <summary>
        /// Initializes the play state with the board configuration and moves to it.
        /// </summary>
        public void OnClick()
        {
            var stateManager = StateManager.Instance;
            var nextStateType = chooseBoardGameState.NextStateType;

            if (nextStateType == StateType.LOBBY_GAME_STATE)
            {
                //Because if the next state is lobby (not play game state), we need to initialize it.
                stateManager.LobbyGameState.ConfigurationType = boardType;
            }
            else
            {
                (stateManager.GetState(nextStateType) as PlayGameState).ConfigurationType = boardType;
            }

            stateManager.ChangeState(nextStateType);
        }
    }
}