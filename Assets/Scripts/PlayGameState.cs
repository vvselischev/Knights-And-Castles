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
        DRAW,
        NONE
    }

    public abstract class PlayGameState : MonoBehaviour, IGameState
    {
        [SerializeField] protected PlayMenu playMenu;
        [SerializeField] protected TurnManager turnManager;
        [SerializeField] protected Timer timer;
        [SerializeField] protected BoardFactory boardFactory;
        [SerializeField] protected ArmyText armyText;
        [SerializeField] protected LerpedText lerpedText;
        [SerializeField] protected StateManager stateManager;
        [SerializeField] protected ExitListener exitListener;
        [SerializeField] protected CheckeredButtonBoard board;
        [SerializeField] protected InputListener inputListener;
        [SerializeField] protected StateType playMode;
        
        protected BlockBoardStorage boardStorage;
        protected ControllerManager controllerManager;
        protected BoardManager boardManager;
        public BoardType ConfigurationType { get; set; }
        private MenuActivator menuActivator = MenuActivator.Instance;

        private const int MAX_TURNS = 10000;

        private int playedTurns;

        public virtual void InvokeState()
        {
            SetupGame();
            boardFactory.FillBoardStorageRandomly(boardStorage);
            
            board.SetInputListener(inputListener);
            var firstController =
                new UserController(PlayerType.FIRST, boardStorage, boardFactory, this, armyText);
            var secondController =
                new UserController(PlayerType.SECOND, boardStorage, boardFactory, this, armyText);
            
            controllerManager = new ControllerManager(firstController, secondController);
            inputListener.Initialize(controllerManager);
            
            playMenu.Initialize(boardManager, inputListener);
            turnManager.Initialize(boardManager, controllerManager);            
            
            InitNewRound();
        }

        protected void SetupGame()
        {
            menuActivator.OpenMenu(playMenu);

            lerpedText.FinishedLerp += CloseGame;
            
            boardStorage = boardFactory.CreateEmptyStorage(ConfigurationType, out boardManager);
            
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
            lerpedText.FinishedLerp -= CloseGame;
            exitListener.Disable();
            boardStorage.Reset();
        }

        protected virtual void InitNewRound()
        {
            playedTurns = 0;

            armyText.Init();

            turnManager.SetTurn(GetFirstTurn());
            timer.StartTimer();

            //Disable or enable UI in child classes!
        }

        public void OnFinishTurn()
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

        protected virtual TurnType GetFirstTurn()
        {
            //Default:
            return TurnType.FIRST;
        }
    }
}