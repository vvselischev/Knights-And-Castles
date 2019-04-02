using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        public BoardFactory boardFactory;
        public ControllerManager controllerManager;
        public ArmyText armyText;
        public UIManager uiManager;
        public StateManager stateManager;
        public ExitListener exitListener;

        public const int MAX_TURNS = 1000;

        private int playedTurns;

        public virtual void InvokeState()
        {
            menuActivator.OpenMenu(playMenu);
            
            storage.Reset();
            boardFactory.Initialize(this);
            boardFactory.FillBoardStorageRandomly();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST,  storage, boardFactory, this, armyText);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, storage, boardFactory,this, armyText);
            
            playedTurns = 0;
            
            timer.OnFinish += ChangeTurn;

            exitListener.Enable();
            exitListener.OnExitClicked += ExitGame;
            InitNewGame();
        }

        public virtual void CloseState()
        {
            menuActivator.CloseMenu();
            timer.OnFinish -= ChangeTurn;
            exitListener.OnExitClicked -= ExitGame;
            exitListener.Disable();
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

        public virtual void OnFinishGame(ResultType resultType)
        {
            timer.StopTimer();
            turnManager.SetTurn(TurnType.RESULT);
            uiManager.FinishedLerp += () =>
            {
                //Here move to the result state
                //Now by default we go to the start menu
                stateManager.ChangeState(StateType.START_GAME_STATE);
            };
        }

        protected virtual void ExitGame()
        {
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