using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum ResultType
    {
        FIRST_WIN,
        SECOND_WIN,
        DRAW,
    }

    public enum UserResultType
    {
        WIN,
        LOSE,
        DRAW
    }

    public abstract class PlayGameState : MonoBehaviour, IGameState
    {
        public PlayMenu playMenu;
        public TurnManager turnManager;
        public Timer timer;
        public BoardFactory boardFactory;
        public ControllerManager controllerManager;
        public ArmyText armyText;
        public UIManager uiManager;
        public StateManager stateManager;
        public ExitListener exitListener;
        public CheckeredButtonBoard board;
        public BoardManager boardManager;
        public InputListener inputListener;
        public BoardType configurationType;
        public StateType stateType;
        
        protected BlockBoardStorage storage;
        private MenuActivator menuActivator = MenuActivator.GetInstance();

        private const int MAX_TURNS = 10000;

        private int playedTurns;

        public virtual void InvokeState()
        {
            SetupGame();
            boardFactory.FillBoardStorageRandomly(storage);
            
            board.SetInputListener(inputListener);
            controllerManager.firstController =
                new UserController(PlayerType.FIRST, storage, boardFactory, this, armyText);
            controllerManager.secondController =
                new UserController(PlayerType.SECOND, storage, boardFactory, this, armyText);
            
            InitNewGame();
        }

        protected void SetupGame()
        {
            menuActivator.OpenMenu(playMenu);

            uiManager.FinishedLerp += CloseGame;
            
            storage = boardFactory.CreateEmptyStorage(configurationType);

            boardManager.Initialize(storage, boardFactory.Configuration.FirstStartBlock,
                boardFactory.Configuration.SecondStartBlock);
            
            playedTurns = 0;

            timer.OnFinish += ChangeTurn;

            exitListener.Enable();
            exitListener.OnExitClicked += ExitGame;
        }

        public virtual void CloseState()
        {
            menuActivator.CloseMenu();
            timer.OnFinish -= ChangeTurn;
            exitListener.OnExitClicked -= ExitGame;
            uiManager.FinishedLerp -= CloseGame;
            exitListener.Disable();
            storage.Reset();
        }

        protected virtual void InitNewRound()
        {
            playedTurns = 0;

            armyText.Init();

            turnManager.SetTurn(GetFirstTurn());
            timer.StartTimer();

            //Disable or enable UI in child classes!
        }

        public void OnFinishTurn(PlayerType finishedType)
        {
            Debug.Log("Finished turn");
            timer.StopTimer();
            ChangeTurn();
        }

        /// <summary>
        /// Called immediately after game finishes.
        /// Children are supposed to perform an appropriate string lerp.
        /// </summary>
        public virtual void OnFinishGame(ResultType _)
        {
            timer.StopTimer();
            turnManager.SetTurn(TurnType.RESULT);
        }

        /// <summary>
        /// Called after string lerp is finished.
        /// Supposed to move to the next state.
        /// Do not forget to initialize ResultGameState if you move to it.
        /// </summary>
        protected abstract void CloseGame();

        protected virtual void ExitGame()
        {
            timer.StopTimer();
            turnManager.SetTurn(TurnType.RESULT);
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }
        
        protected virtual void ChangeTurn()
        {
            if (playedTurns == MAX_TURNS)
            {
                turnManager.SetTurn(TurnType.RESULT);
                timer.StopTimer();
            }
            else
            {
                turnManager.SetNextTurn();
                timer.StartTimer();
            }
            playedTurns++; 

            //Further behaviour should be specified in child classes.
        }

        protected void InitNewGame()
        {
            InitNewRound();
        }

        protected virtual TurnType GetFirstTurn()
        {
            //TODO: make random
            return TurnType.FIRST;
        }
    }
}