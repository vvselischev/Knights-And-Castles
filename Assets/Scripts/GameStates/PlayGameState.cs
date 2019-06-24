using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Result type from the game point of view.
    /// </summary>
    public enum ResultType
    {
        FIRST_WIN,
        SECOND_WIN,
        DRAW
    }

    /// <summary>
    /// Result type from user's point of view.
    /// </summary>
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
            boardFactory.FillBoardStorageRandomly(boardStorage, ConfigurationType, boardManager);
            
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

        /// <summary>
        /// Common setup for all child classes.
        /// </summary>
        protected void SetupGame()
        {
            menuActivator.OpenMenu(playMenu);

            lerpedText.FinishedLerp += CloseGame;
            
            boardStorage = boardFactory.CreateEmptyBlockStorage(ConfigurationType, out boardManager);
            
            playedTurns = 0;

            timer.OnFinish += ChangeTurn;

            exitListener.Enable();
            exitListener.OnExitClicked += OnBackButtonPressed;
        }

        public virtual void CloseState()
        {
            menuActivator.CloseMenu();
            timer.OnFinish -= ChangeTurn;
            exitListener.OnExitClicked -= OnBackButtonPressed;
            lerpedText.FinishedLerp -= CloseGame;
            exitListener.Disable();
            boardStorage.Reset();
        }

        /// <summary>
        /// Actual start of the round.
        /// </summary>
        protected virtual void InitNewRound()
        {
            playedTurns = 0;

            armyText.Init();

            turnManager.SetTurn(GetFirstTurn());
            timer.StartTimer();

            //Disable or enable UI in child classes!
        }

        /// <summary>
        /// Subscribed on the current controller.
        /// Changed turn and resets timer.
        /// </summary>
        public void OnFinishTurn()
        {
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

        protected virtual void OnBackButtonPressed()
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