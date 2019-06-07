using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Class for convenient change between game states.
    /// Automatically calls Invoke and Close methods.
    /// Singleton, because unique and could be called from different places.
    /// </summary>
    public class StateManager : MonoBehaviour
    {
        public IGameState CurrentState { get; private set; }
        public static StateManager Instance { get; private set; }

        // Made public for convenient DI in the editor and since some objects need them to initialize.
        public StartGameState startGameState;
        public NetworkPlayGameState networkPlayGameState;
        public AIPlayGameState aiPlayGameState;
        public OneDeviceMultiplayerGameState oneDeviceMultiplayerGameState;
        public LobbyGameState lobbyGameState;
        public ChooseBoardGameState chooseBoardGameState;
        public ResultGameState resultGameState;
        public InfoGameState infoGameState;
        
        private Dictionary<StateType, IGameState> states;

        /// <summary>
        /// Like an entry point of application.
        /// Moves to the start state.
        /// </summary>
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Instance = this;
            Initialize();
            ChangeState(StateType.START_GAME_STATE);
        }

        /// <summary>
        /// Returns the game state by its type.
        /// Returned value is packed in IGameState. However, the correspondence is guaranteed.
        /// </summary>
        public IGameState GetState(StateType stateType)
        {
            return states[stateType];
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
                {StateType.ONE_DEVICE_MULTIPLAYER_STATE, oneDeviceMultiplayerGameState},
                {StateType.RESULT_GAME_STATE, resultGameState},
                {StateType.INFO_GAME_STATE, infoGameState}
            };
        }

        /// <summary>
        /// Closes current state (if exists) and invokes the given state.
        /// </summary>
        public void ChangeState(StateType newStateType)
        {
            CurrentState?.CloseState();

            var newState = states[newStateType];
            CurrentState = newState;
            newState.InvokeState();
        }
    }
}