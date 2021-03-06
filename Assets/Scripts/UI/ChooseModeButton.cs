﻿using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represent a button for choosing a play mode on the start menu.
    /// Initializes ChooseBoard game state and switches the game to it.
    /// </summary>
    public class ChooseModeButton : MonoBehaviour
    {
        [SerializeField] private StateType nextState;

        /// <summary>
        /// Remembers the chosen play game state and moves to the board mode choosing.
        /// </summary>
        public void OnClick()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChooseBoardGameState.NextStateType = nextState;
            stateManager.ChangeState(StateType.CHOOSE_BOARD_GAME_STATE);
        }
    }
}