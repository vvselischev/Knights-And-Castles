using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class InputListener : MonoBehaviour
    {
        public ControllerManager controllerManager;

        public void ProcessBoardClick(BoardButton boardButton)
        {
            controllerManager.currentController.OnButtonClick(boardButton);
        }

        public void ProcessFinishTurnClick()
        {
            controllerManager.FinishTurn();
        }
    }
}