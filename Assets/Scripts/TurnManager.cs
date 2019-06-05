using UnityEngine;

namespace Assets.Scripts
{
    public enum TurnType
    {
        FIRST,
        SECOND,
        RESULT
    }

    public class TurnManager : MonoBehaviour
    {
        private ControllerManager controllerManager;
        private BoardManager boardManager;
        [SerializeField] private GameObject firstIcon;
        [SerializeField] private GameObject secondIcon;

        public TurnType CurrentTurn { get; private set; }

        public void Initialize(BoardManager boardManager, ControllerManager controllerManager)
        {
            this.boardManager = boardManager;
            this.controllerManager = controllerManager;
        }
        
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
