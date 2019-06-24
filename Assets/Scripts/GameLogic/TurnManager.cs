using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Determines the current state of the Turn Manager
    /// </summary>
    public enum TurnType
    {
        FIRST,
        SECOND,
        RESULT
    }

    /// <summary>
    /// A class for a convenient turn change.
    /// Changes active controllers and player icons in the left-up corner.
    /// Saves and restores player active blocks using BoardManager:
    /// When a turn comes to the player, activates the block that was the last active during this player's last turn
    /// or default block set in Board Manager, if is the first turn.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        /// <summary>
        /// To change an active controller.
        /// </summary>
        private ControllerManager controllerManager;
        /// <summary>
        /// To save and restore player blocks.
        /// </summary>
        private BoardManager boardManager;
        
        /// <summary>
        /// Icons for both players in the left-up corner of the ui.
        /// </summary>
        [SerializeField] private GameObject firstIcon;
        [SerializeField] private GameObject secondIcon;

        /// <summary>
        /// Returns the current turn.
        /// </summary>
        public TurnType CurrentTurn { get; private set; }

        /// <summary>
        /// Should be called anytime when boardManager or controllerManager are changed.
        /// </summary>
        public void Initialize(BoardManager boardManager, ControllerManager controllerManager)
        {
            this.boardManager = boardManager;
            this.controllerManager = controllerManager;
        }
        
        /// <summary>
        /// Sets new turn.
        /// Saves the current player block and disables its controller.
        /// Restores the next-player's block and enables its controller.
        /// Activates the corresponding icon.
        /// If next turn if Result, deactivates both controllers.
        /// </summary>
        public void SetTurn(TurnType turn)
        {
            CurrentTurn = turn;
            controllerManager.SetCurrentController(turn);
            if (turn == TurnType.FIRST)
            {
                boardManager.SavePlayerBlock(TurnType.SECOND);
                controllerManager.DisableController(TurnType.SECOND);
                
                boardManager.SetPlayerBlockActive(TurnType.FIRST);
                controllerManager.EnableController(TurnType.FIRST);
                
                secondIcon.SetActive(false);
                firstIcon.SetActive(true);
            }
            else if (turn == TurnType.SECOND)
            {
                boardManager.SavePlayerBlock(TurnType.FIRST);
                controllerManager.EnableController(TurnType.SECOND);
                
                boardManager.SetPlayerBlockActive(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
                
                secondIcon.SetActive(true);
                firstIcon.SetActive(false);
            }
            else if (turn == TurnType.RESULT)
            {
                controllerManager.DeactivateAll();
            }
        }

        /// <summary>
        /// Sets new turn, opposite to current. If current is none, sets First.
        /// </summary>
        public void SetNextTurn()
        {
            if (CurrentTurn == TurnType.FIRST)
            {
                SetTurn(TurnType.SECOND);
            }
            else
            {
                SetTurn(TurnType.FIRST);
            }
        }
    }
}
