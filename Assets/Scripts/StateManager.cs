using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum StateType
    {
        START_GAME_STATE,
        LOBBY_GAME_STATE,
        NETWORK_GAME_STATE,
        AI_GAME_STATE,
        ONE_DEVICE_MULTIPLAYER_STATE
    }

    public class StateManager : MonoBehaviour
    {
        private IGameState currentState;

        public StartGameState StartState;
        public NetworkPlayGameState networkPlayGameState;
        public AIPlayGameState aiPlayGameState;
        public OneDeviceMultiplayerGameState oneDeviceMultiplayerGameState;
        public LobbyGameState LobbyGameState;
        
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
            states.Add(StateType.LOBBY_GAME_STATE, LobbyGameState);
            states.Add(StateType.NETWORK_GAME_STATE, networkPlayGameState);
            states.Add(StateType.AI_GAME_STATE, aiPlayGameState);
            states.Add(StateType.ONE_DEVICE_MULTIPLAYER_STATE, oneDeviceMultiplayerGameState);
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