using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Play mode with bot.
    /// </summary>
    public class AIPlayGameState : PlayGameState
    {
        private AIPlayer aiPlayer;
        private UserResultType resultType;

        public override void InvokeState()
        {
            base.InvokeState();
            //Let AI be the Type.Second (however, it can be the first to move)
            aiPlayer = new AIPlayer(controllerManager.SecondController, PlayerType.SECOND, boardStorage, 
                boardManager, inputListener);
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();

            if (turnManager.CurrentTurn == TurnType.SECOND)
            {
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
                lerpedText.PerformLerpString("You win!", Color.green);
                this.resultType = UserResultType.WIN;
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                lerpedText.PerformLerpString("You lose...", Color.red);
                this.resultType = UserResultType.LOSE;
            }
            else if (resultType == ResultType.DRAW)
            {
                lerpedText.PerformLerpString("Draw", Color.blue);
                this.resultType = UserResultType.DRAW;
            }
        }

        protected override void CloseGame()
        {
            stateManager.resultGameState.Initialize(resultType, playMode);
            stateManager.ChangeState(StateType.RESULT_GAME_STATE);
        }

        protected override void InitNewRound()
        {
            base.InitNewRound();
            if (turnManager.CurrentTurn == TurnType.FIRST)
            {
                playMenu.EnableUI();
            }
            else
            {
                playMenu.DisableUI();
            }
        }
    }
}