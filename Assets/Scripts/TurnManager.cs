using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO: change name?
    public enum TurnType
    {
        FIRST,
        SECOND,
        RESULT
    }

    public class TurnManager : MonoBehaviour
    {
        public ControllerManager controllerManager;
        public BoardManager boardManager;
        public GameObject firstIcon;
        public GameObject secondIcon;

        private TurnType currentTurn;

        public TurnType CurrentTurn => currentTurn;
        
        public void SetTurn(TurnType turn)
        {
            currentTurn = turn;
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
                controllerManager.DisableController(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
            }
        }

        public void SetNextTurn()
        {
            if (currentTurn == TurnType.FIRST)
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
