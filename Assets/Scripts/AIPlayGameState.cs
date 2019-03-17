using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayGameState : PlayGameState
    {
        public AIPlayer aiPlayer;
        public System.Random random = new System.Random();
        public override void InvokeState()
        {
            base.InvokeState();
            //Let AI be the Type.Second (however, it can be the first to move)
            aiPlayer.Initialize(controllerManager.SecondController, PlayerType.SECOND);
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();

            if (turnManager.CurrentTurn == TurnType.SECOND)
            {
                //TODO: lock not only the board but also buttons (change turn, split)

                //lock board and ui for user
                playMenu.DisableUI();

                //Activate AI and wait for it to perform move
                aiPlayer.Activate();
                //Don't write anything after this statement! (AI may perform its move immediately)
            }
            else if (turnManager.CurrentTurn == TurnType.FIRST)
            {
                //unlock board and ui for user
                playMenu.EnableUI();
            }
        }

        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            if (resultType == ResultType.FIRST_WIN)
            {
                uiManager.PerformLerpString("You win!", Color.green);
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                uiManager.PerformLerpString("You lose...", Color.red);
            }
            else if (resultType == ResultType.DRAW)
            {
                uiManager.PerformLerpString("Draw", Color.blue);
            }
        }
    }
}