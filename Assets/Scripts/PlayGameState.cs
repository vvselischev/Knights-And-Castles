using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayGameState : MonoBehaviour, IGameState
    {
        protected readonly MenuActivator menuActivator = MenuActivator.GetInstance();
        public PlayMenu playMenu;
        public CheckeredButtonBoard board;
        public BoardStorage storage;
        public TurnManager turnManager;
        public Timer timer;
        public RoundScore roundScore;
        [FormerlySerializedAs("boardCreator")] public BoardFactory boardFactory;
        public ControllerManager controllerManager;
        public ArmyTextManager armyTextManager;

        public const int MAX_TURNS = 1000;

        private int currentRound = 0;
        private int playedTurns = 0;

        public virtual void InvokeState()
        {
            menuActivator.OpenMenu(playMenu);
            
            storage.Reset();
            boardFactory.FillBoardStorageRandomly();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST,  storage, boardFactory, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, storage, boardFactory,this);
            InitNewGame();
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
        }

        private void InitNewRound()
        {
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