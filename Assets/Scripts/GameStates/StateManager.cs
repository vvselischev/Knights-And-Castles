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
        /// <summary>
        /// A dictionary to get the state by its type.
        /// </summary>
        private Dictionary<StateType, IGameState> states;

        /// <summary>
        /// States fields for convenient DI in the editor.
        /// </summary>
        [SerializeField] private StartGameState startGameState;
        [SerializeField] private NetworkPlayGameState networkPlayGameState;
        [SerializeField] private AIPlayGameState aiPlayGameState;
        [SerializeField] private OneDeviceMultiplayerGameState oneDeviceMultiplayerGameState;
        [SerializeField] private LobbyGameState lobbyGameState;
        [SerializeField] private ChooseBoardGameState chooseBoardGameState;
        [SerializeField] private ResultGameState resultGameState;
        [SerializeField] private InfoGameState infoGameState;
        [SerializeField] private TutorialGameState tutorialGameState;
        
        //States are made properties since some objects need them to initialize.
        public StartGameState StartGameState => startGameState;
        public NetworkPlayGameState NetworkPlayGameState => networkPlayGameState;
        public AIPlayGameState AIPlayGameState => aiPlayGameState;
        public OneDeviceMultiplayerGameState OneDeviceMultiplayerGameState => oneDeviceMultiplayerGameState;
        public LobbyGameState LobbyGameState => lobbyGameState;
        public ChooseBoardGameState ChooseBoardGameState => chooseBoardGameState;
        public ResultGameState ResultGameState => resultGameState;
        public InfoGameState InfoGameState => infoGameState;
        public TutorialGameState TutorialGameState => tutorialGameState;
        
        public IGameState CurrentState { get; private set; }
        public static StateManager Instance { get; private set; }
        
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

        /// <summary>
        /// Initializes the dictionary of states.
        /// States are matched with their types.
        /// </summary>
        private void Initialize()
        {
            states = new Dictionary<StateType, IGameState>
            {
                {StateType.START_GAME_STATE, StartGameState},
                {StateType.CHOOSE_BOARD_GAME_STATE, ChooseBoardGameState},
                {StateType.LOBBY_GAME_STATE, LobbyGameState},
                {StateType.NETWORK_GAME_STATE, NetworkPlayGameState},
                {StateType.AI_GAME_STATE, AIPlayGameState},
                {StateType.ONE_DEVICE_MULTIPLAYER_STATE, OneDeviceMultiplayerGameState},
                {StateType.RESULT_GAME_STATE, ResultGameState},
                {StateType.INFO_GAME_STATE, InfoGameState},
                {StateType.TUTORIAL_GAME_STATE, TutorialGameState}
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