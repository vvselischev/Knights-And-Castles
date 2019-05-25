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
        public IGameState CurrentState { get; private set; }

        public static StateManager Instance { get; private set; }

        public StartGameState startGameState;
        public NetworkPlayGameState networkPlayGameState;
        public AIPlayGameState aiPlayGameState;
        public OneDeviceMultiplayerGameState oneDeviceMultiplayerGameState;
        public LobbyGameState lobbyGameState;
        
        private static Dictionary<StateType, IGameState> states;

        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("Start");

            Instance = this;
            Initialize();
            ChangeState(StateType.START_GAME_STATE);
        }

        private void Initialize()
        {
            states = new Dictionary<StateType, IGameState>
            {
                {StateType.START_GAME_STATE, startGameState},
                {StateType.LOBBY_GAME_STATE, lobbyGameState},
                {StateType.NETWORK_GAME_STATE, networkPlayGameState},
                {StateType.AI_GAME_STATE, aiPlayGameState},
                {StateType.ONE_DEVICE_MULTIPLAYER_STATE, oneDeviceMultiplayerGameState}
            };
        }

        public void ChangeState(StateType newStateType)
        {
            Debug.Log("Current State: " + CurrentState);
            if (CurrentState != null)
            {
                Debug.Log("Closing " + CurrentState);
                CurrentState.CloseState();
            }

            IGameState newState = states[newStateType];
            CurrentState = newState;
            Debug.Log("New state: " + CurrentState);
            newState.InvokeState();
        }
    }
}