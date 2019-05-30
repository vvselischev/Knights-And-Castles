using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class OneDeviceMultiplayerGameState : PlayGameState
    {
        public override void InvokeState()
        {
            base.InvokeState();
            storage.EnableBoardButtons();
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            storage.InvertBoard();
        }
        
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            if (resultType == ResultType.FIRST_WIN)
            {
                uiManager.PerformLerpString("First player wins!", Color.green);
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                uiManager.PerformLerpString("Second player wins!", Color.green);
            }
            else if (resultType == ResultType.DRAW)
            {
                uiManager.PerformLerpString("Draw", Color.blue);
            }
        }

        protected override void CloseGame()
        {
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

        protected override void InitNewRound()
        {
            base.InitNewRound();
            playMenu.EnableUI();
        }
    }
}