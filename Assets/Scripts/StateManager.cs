﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
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
        public ResultGameState resultGameState;
        public InfoGameState infoGameState;
        
        private Dictionary<StateType, IGameState> states;

        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Instance = this;
            Initialize();
            ChangeState(StateType.START_GAME_STATE);
        }

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

        public void ChangeState(StateType newStateType)
        {
            if (CurrentState != null)
            {
                CurrentState.CloseState();
            }

            var newState = states[newStateType];
            CurrentState = newState;
            newState.InvokeState();
        }
    }
}