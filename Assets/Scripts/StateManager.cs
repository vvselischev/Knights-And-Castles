using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum StateType
    {
        START_GAME_STATE,
        CHOOSE_BOARD_GAME_STATE,
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
        public ChooseBoardGameState chooseBoardGameState;
        
        public Dictionary<StateType, IGameState> states;

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
                {StateType.CHOOSE_BOARD_GAME_STATE, chooseBoardGameState},
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