using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Class of play UI. Activates and deactivates buttons.
    /// </summary>
    public class PlayMenu : MonoBehaviour, IMenu
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private FinishTurnButton finishTurnButton;
        [SerializeField] private SplitButton splitButton;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ArmyText armyText;
        [SerializeField] private RoundEffects roundEffects;

        public void Initialize(BoardManager boardManager, InputListener inputListener)
        {
            this.boardManager = boardManager;
            finishTurnButton.InputListener = inputListener;
            splitButton.InputListener = inputListener;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(true);
            canvas.enabled = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            canvas.enabled = false;
        }

        /// <summary>
        /// Disables the UI for the user.
        /// All buttons are locked and do not process an input.
        /// Effects for the particular user are turned off.
        /// </summary>
        public void DisableUI()
        {
            boardManager.GetCurrentBlock().DisableBoardButtons();
            finishTurnButton.Lock();
            splitButton.Lock();
            armyText.Disable();
            roundEffects.DisablePlayerEffects();
        }

        /// <summary>
        /// Enables the UI for the user.
        /// All buttons are unlocked and do process an input.
        /// Effects for the particular user are turned on.
        /// </summary>
        public void EnableUI()
        {
            boardManager.GetCurrentBlock().EnableBoardButtons();
            finishTurnButton.Unlock();
            splitButton.Unlock();
            armyText.Enable();
            roundEffects.EnablePlayerEffects();
        }
    }
}