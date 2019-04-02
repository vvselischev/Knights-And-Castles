using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO: change name
    public enum TurnType
    {
        FIRST,
        SECOND,
        RESULT
    }

    public class TurnManager : MonoBehaviour
    {
        public ControllerManager controllerManager;
        public GameObject FirstIcon;
        public GameObject SecondIcon;

        private TurnType currentTurn;

        public TurnType CurrentTurn => currentTurn;

        public void SetTurn(TurnType turn)
        {
            currentTurn = turn;
            controllerManager.SetCurrentController(turn);
            if (turn == TurnType.FIRST)
            {
                controllerManager.DisableController(TurnType.SECOND);
                controllerManager.EnableController(TurnType.FIRST);
                SecondIcon.SetActive(false);
                FirstIcon.SetActive(true);
            }
            else if (turn == TurnType.SECOND)
            {
                controllerManager.EnableController(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
                SecondIcon.SetActive(true);
                FirstIcon.SetActive(false);
            }
            else if (turn == TurnType.RESULT)
            {
                controllerManager.DisableController(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
                //SecondIcon.SetActive(false);
                //FirstIcon.SetActive(false);
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
