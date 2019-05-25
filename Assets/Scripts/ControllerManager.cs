using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class ControllerManager : MonoBehaviour
    {
        public UserController currentController;
        public UserController firstController;
        public UserController secondController;

        public void SetCurrentController(TurnType type)
        {
            if (type == TurnType.SECOND)
            {
                currentController = secondController;
            }
            else if (type == TurnType.FIRST)
            {
                currentController = firstController;
            }
        }

        public void EnableController(TurnType type)
        {
            if (type == TurnType.FIRST)
            {
                firstController.Enable();
            }
            else
            {
                secondController.Enable();
            }
        }

        public void DisableController(TurnType type)
        {
            if (type == TurnType.FIRST)
            {
                firstController.Disable();
            }
            else
            {
                secondController.Disable();
            }
        }

        public bool HasActiveController()
        {
            return currentController != null;
        }

        public void DeactivateAll()
        {
            currentController = null;
            firstController.Disable();
            secondController.Disable();
        }
    }
}