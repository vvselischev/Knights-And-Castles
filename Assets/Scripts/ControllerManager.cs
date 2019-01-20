using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class ControllerManager : MonoBehaviour
    {
        public UserController currentController;
        public UserController FirstController;
        public UserController SecondController;

        public void SetCurrentController(TurnType type)
        {
            if (type == TurnType.SECOND)
            {
                currentController = SecondController;
            }
            else
            {
                currentController = FirstController;
            }
        }

        public void FinishTurn()
        {
            currentController.FinishTurn();
        }

        public void InitRound()
        {
            
        }

        public void EnableController(TurnType type)
        {
            if (type == TurnType.FIRST)
            {
                FirstController.Enable();
            }
            else
            {
                SecondController.Enable();
            }
        }

        public void DisableController(TurnType type)
        {
            if (type == TurnType.FIRST)
            {
                FirstController.Disable();
            }
            else
            {
                SecondController.Disable();
            }
        }
    }
}