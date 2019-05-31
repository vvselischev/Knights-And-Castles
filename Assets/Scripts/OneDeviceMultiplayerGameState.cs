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
            boardStorage.EnableBoardButtons();
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            boardStorage.InvertBoard();
        }
        
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            if (resultType == ResultType.FIRST_WIN)
            {
                lerpedText.PerformLerpString("First player wins!", Color.green);
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                lerpedText.PerformLerpString("Second player wins!", Color.green);
            }
            else if (resultType == ResultType.DRAW)
            {
                lerpedText.PerformLerpString("Draw", Color.blue);
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