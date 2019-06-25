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

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void InvokeState()
        {
            base.InvokeState();
            //Let AI be the Type.Second (however, it can be the first to move)
            aiPlayer = new AIPlayer(controllerManager.SecondController, PlayerType.SECOND, boardStorage, 
                boardManager, inputListener);
        }

        /// <summary>
        /// Changes the turn to the opponent's.
        /// If the next turn is bot, then the board is disabled and enabled otherwise.
        /// </summary>
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

        /// <summary>
        /// Starts displaying the string with the game result and saves it for further processing.
        /// </summary>
        public override void OnFinishGame(ResultType resultType)
        {
            base.OnFinishGame(resultType);
            if (resultType == ResultType.FIRST_WIN)
            {
                //User wins
                lerpedText.PerformLerpString("You win!", Color.green);
                this.resultType = UserResultType.WIN;
            }
            else if (resultType == ResultType.SECOND_WIN)
            {
                //Bot wins
                lerpedText.PerformLerpString("You lose...", Color.red);
                this.resultType = UserResultType.LOSE;
            }
            else if (resultType == ResultType.DRAW)
            {
                lerpedText.PerformLerpString("Draw", Color.blue);
                this.resultType = UserResultType.DRAW;
            }
        }

        /// <summary>
        /// Method is called when the result string displaying is finished.
        /// Initializes the result state and moves to it.
        /// </summary>
        protected override void CloseGame()
        {
            stateManager.ResultGameState.Initialize(resultType, playMode);
            stateManager.ChangeState(StateType.RESULT_GAME_STATE);
        }

        /// <summary>
        /// Initializes new round.
        /// Enables the UI of the user with the current (first) move.
        /// </summary>
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