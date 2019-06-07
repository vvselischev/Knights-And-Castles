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
    /// Changes active controllers, player icons in the left-up corner.
    /// Performs logic to save and restore player active blocks via BoardManager.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        private ControllerManager controllerManager;
        private BoardManager boardManager;
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
