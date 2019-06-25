using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Play mode for 2 players on one device.
    /// Simply inverts the board after each turn.
    /// </summary>
    public class OneDeviceMultiplayerGameState : PlayGameState
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
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
        
        /// <summary>
        /// Displays the result text, increasing in size.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void CloseGame()
        {
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void InitNewRound()
        {
            base.InitNewRound();
            playMenu.EnableUI();
        }
    }
}