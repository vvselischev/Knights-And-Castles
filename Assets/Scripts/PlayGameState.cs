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
        NONE
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

        protected BlockBoardStorage storage;
        protected DataService dataService;
        private MenuActivator menuActivator = MenuActivator.GetInstance();

        public const int MAX_TURNS = 10000;

        private int playedTurns;

        public virtual void InvokeState()
        {
            SetupGame();
            boardFactory.FillBoardStorageRandomly();

            controllerManager.FirstController =
                new UserController(PlayerType.FIRST, storage, boardFactory, this, armyText);
            controllerManager.SecondController =
                new UserController(PlayerType.SECOND, storage, boardFactory, this, armyText);

            InitNewGame();
        }

        protected void SetupGame()
        {
            menuActivator.OpenMenu(playMenu);
            dataService = new DataService("record_database.db");

            uiManager.FinishedLerp += SetupFinishGame;
            
            storage = boardFactory.Initialize();

            turnManager.Initialize(storage);
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
            uiManager.FinishedLerp -= SetupFinishGame;
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

        public virtual void OnFinishGame(ResultType resultType)
        {
            timer.StopTimer();
            turnManager.SetTurn(TurnType.RESULT);
        }

        private void SetupFinishGame()
        {
            //Here move to the result state
            //Now by default we go to the start menu
            //stateManager.ChangeState(StateType.START_GAME_STATE);

            var records = dataService.GetUserRecords("Vlad");
            int total = 0;

            foreach (var value in records)
            {
                playMenu.playDebugText.text += value + "\n";
                total++;
            }

            playMenu.playDebugText.text += "Total: " + total + '\n';

            dataService.AddRecord(new Record
            {
                Login = "Vlad",
                WinsBot = total + 10,
            });
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