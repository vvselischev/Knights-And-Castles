using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayGameState : PlayGameState
    {
        public AIPlayer aiPlayer;

        public override void InvokeState()
        {
            base.InvokeState();
            //Let AI be the Type.Second (however, it can be the first to move)
            aiPlayer.Initialize(controllerManager.SecondController);
        }
        public override void ChangeTurn()
        {
            base.ChangeTurn();

            if (turnManager.CurrentTurn == TurnType.SECOND)
            {
                //lock board for user
                board.DisableBoard();
                
                //Activate AI and wait for it to perform move
                aiPlayer.Activate();
                //Don't write anything after this statement! (AI may perform its move immidiately)
            }
            else if (turnManager.CurrentTurn == TurnType.FIRST)
            {
                //unlock board for user
                board.EnableBoard();
            }
        }
    }
}