using UnityEngine;
using System.Collections;
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
        NONE
    }
    
    public abstract class PlayGameState : MonoBehaviour, IGameState
    {
        protected readonly MenuActivator menuActivator = MenuActivator.GetInstance();
        public PlayMenu playMenu;
        public CheckeredButtonBoard board;
        public BoardStorage storage;
        public TurnManager turnManager;
        public Timer timer;
        public RoundScore roundScore;
        public BoardFactory boardFactory;
        public ControllerManager controllerManager;
        public ArmyTextManager armyTextManager;
        public UIManager uiManager;
        public StateManager stateManager;

        public const int MAX_TURNS = 1000;

        private int currentRound;
        private int playedTurns;

        public virtual void InvokeState()
        {
            menuActivator.OpenMenu(playMenu);
            
            storage.Reset();
            boardFactory.Initialize(this);
            boardFactory.FillBoardStorageRandomly();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST,  storage, boardFactory, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, storage, boardFactory,this);

            currentRound = 0;
            playedTurns = 0;
            
            InitNewGame();
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
        }

        private void InitNewRound()
        {
            //TODO: if we restart the game, board sometimes is inverted??!!
            
            //buttonListener.Reset();
            turnManager.InitRound();
            playedTurns = 0;
            currentRound++;

            playMenu.UpdateRoundText(currentRound);

            if (currentRound > 5)
            {
                //end game
                return;
            }

            if (currentRound == 1)
            {
                turnManager.SetTurn(GetFirstTurn());
            }
            else if (currentRound > 1)
            {
                turnManager.SetNextTurn();
            }

            timer.StartTimer();
            armyTextManager.Init();
        }

        public void OnFinishTurn(PlayerType finishedType)
        {
            Debug.Log("Finished turn");
            //buttonListener.Reset();
            ChangeTurn();
        }

        public void OnFinishRound()
        {
            playMenu.UpdateScoreText(roundScore);
            InitNewRound();
        }

        public virtual void OnFinishGame(ResultType resultType)
        {
            uiManager.FinishedLerp += () =>
            {
                //Here move to the result state
                //Now by default we go to the start menu
                stateManager.ChangeState(StateType.START_GAME_STATE);
            };
        }

        protected virtual void ChangeTurn()
        {
            if (playedTurns == MAX_TURNS)
            {
                turnManager.SetTurn(TurnType.RESULT);
                timer.StopTimer();
                //roundJudge.StartJudge();
            }
            else
            {
                turnManager.SetNextTurn();
                timer.StartTimer();
            }
            playedTurns++; 

            //Further behaviour should be specified in child classes.
        }

        protected virtual void InitNewGame()
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