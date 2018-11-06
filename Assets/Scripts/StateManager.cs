using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum StateType
    {
        START_GAME_STATE,
        PLAY_GAME_STATE
    }

    public class StateManager : MonoBehaviour
    {
        private IGameState currentState;

        public StartGameState StartState;
        public PlayGameState PlayState;
        private static Dictionary<StateType, IGameState> states;

        void Awake()
        {
            Debug.Log("Start");

            Initialize();
            ChangeState(StateType.START_GAME_STATE);
        }

        private void Initialize()
        {
            states = new Dictionary<StateType, IGameState>();
            states.Add(StateType.START_GAME_STATE, StartState);
            states.Add(StateType.PLAY_GAME_STATE, PlayState);
        }

        public void ChangeState(StateType newStateType)
        {
            Debug.Log("Current State: " + currentState);
            if (currentState != null)
            {
                Debug.Log("Closing " + currentState);
                currentState.CloseState();
            }

            IGameState newState = states[newStateType];
            currentState = newState;
            Debug.Log("New state: " + currentState);
            newState.InvokeState();
        }
    }
}