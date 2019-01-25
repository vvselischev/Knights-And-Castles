using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayGameState : MonoBehaviour, IGameState
    {
        private readonly MenuActivator menuActivator = MenuActivator.GetInstance();
        public PlayMenu playMenu;
        public CheckeredButtonBoard board;
        public BoardStorage storage;
        public TurnManager turnManager;
        public Timer timer;
        //public RoundJudge roundJudge;
        public InputListener inputListener;
        public RoundScore roundScore;
        public RoundBoardCreator boardCreator;
        public ControllerManager controllerManager;
        public ArmyTextManager armyTextManager;

        public const int MAX_TURNS = 100;

        private int currentRound = 0;
        private int playedTurns = 0;

        public virtual void InvokeState()
        {
            menuActivator.OpenMenu(playMenu);
            //roundJudge.OnFinishJudge += OnFinishRound;
            
            storage.Reset();
            boardCreator.FillBoardStorage();
            
            controllerManager.FirstController = new UserController(PlayerType.FIRST, 
                                boardCreator.FirstArmy, storage, this);
            controllerManager.SecondController = new UserController(PlayerType.SECOND, 
                                boardCreator.SecondArmy, storage, this);
            InitNewGame();
        }

        public void CloseState()
        {
            menuActivator.CloseMenu();
        }

        public void InitNewRound()
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

        public virtual void ChangeTurn()
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

        public void InitNewGame()
        {
            //These line was in legacy code. 
            //Probably, it deletes buttons created for the editor mode since they're not initialized correctly.
            board.DeleteButtons();
            board.CreateButtons();
            InitNewRound();
        }

        private TurnType GetFirstTurn()
        {
            return TurnType.FIRST;
        }
    }
}