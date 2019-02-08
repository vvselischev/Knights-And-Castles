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

        public GameObject MouseUI;
        public GameObject CatUI;
        public GameObject RoundUI;

        public GameObject mouseMover;
        public GameObject MouseIcon;
        public GameObject CatIcon;

        private TurnType currentTurn;

        public TurnType CurrentTurn => currentTurn;

        public void InitRound()
        {
            controllerManager.InitRound();
        }

        private void EnableRound()
        {
            RoundUI.SetActive(true);
        }

        private void DisableRound()
        {
            RoundUI.SetActive(false);
        }

        public void SetTurn(TurnType turn)
        {
            currentTurn = turn;
            controllerManager.SetCurrentController(turn);
            if (turn == TurnType.FIRST)
            {
                EnableRound();
                controllerManager.DisableController(TurnType.SECOND);
                controllerManager.EnableController(TurnType.FIRST);
            }
            else if (turn == TurnType.SECOND)
            {
                EnableRound();
                controllerManager.EnableController(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
            }
            else if (turn == TurnType.RESULT)
            {
                controllerManager.DisableController(TurnType.SECOND);
                controllerManager.DisableController(TurnType.FIRST);
                DisableRound();
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
