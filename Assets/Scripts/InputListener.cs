using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class InputListener : MonoBehaviour
    {
        public ControllerManager controllerManager;

        public virtual void ProcessBoardClick(int x, int y)
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.currentController;
                currentController.OnButtonClick(x, y);
            }
        }

        public virtual void ProcessFinishTurnClick()
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.currentController;
                currentController.FinishTurn();
            }
        }

        public virtual void ProcessSplitButtonClick()
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.currentController;
                currentController.OnSplitButtonClick();
            }
        }
    }
}