using UnityEngine;

namespace Assets.Scripts
{
    public class PlayMenu : MonoBehaviour, IMenu
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private FinishTurnButton finishTurnButton;
        [SerializeField] private SplitButton splitButton;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ArmyText armyText;

        public void Initialize(BoardManager boardManager, InputListener inputListener)
        {
            this.boardManager = boardManager;
            finishTurnButton.inputListener = inputListener;
            splitButton.InputListener = inputListener;
        }
        
        public void Activate()
        {
            gameObject.SetActive(true);
            canvas.enabled = true;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            canvas.enabled = false;
        }

        public void DisableUI()
        {
            boardManager.GetCurrentBlock().DisableBoardButtons();
            finishTurnButton.Lock();
            splitButton.Lock();
            armyText.Disable();
        }

        public void EnableUI()
        {
            boardManager.GetCurrentBlock().EnableBoardButtons();
            finishTurnButton.Unlock();
            splitButton.Unlock();
            armyText.Enable();
        }
    }
}