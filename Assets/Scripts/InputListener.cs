using UnityEngine;

namespace Assets.Scripts
{
    public class InputListener : MonoBehaviour
    {
        public ControllerManager controllerManager;

        public virtual void Initialize(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
        }
        
        public virtual void ProcessBoardClick(int x, int y)
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.CurrentController;
                currentController.OnButtonClick(x, y);
            }
        }

        public virtual void ProcessFinishTurnClick()
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.CurrentController;
                currentController.FinishTurn();
            }
        }

        public virtual void ProcessSplitButtonClick()
        {
            if (controllerManager.HasActiveController())
            {
                UserController currentController = controllerManager.CurrentController;
                currentController.OnSplitButtonClick();
            }
        }
    }
}