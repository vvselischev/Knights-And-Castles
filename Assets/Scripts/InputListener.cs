using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Receives the local input and transfers it to the current controller.
    /// </summary>
    public class InputListener : MonoBehaviour
    {
        private ControllerManager controllerManager;

        /// <summary>
        /// Must be called before usage.
        /// </summary>
        public void Initialize(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
        }
        
        public virtual void ProcessBoardClick(int x, int y)
        {
            if (controllerManager.HasActiveController())
            {
                var currentController = controllerManager.CurrentController;
                currentController.OnButtonClick(x, y);
            }
        }

        public virtual void ProcessFinishTurnClick()
        {
            if (controllerManager.HasActiveController())
            {
                var currentController = controllerManager.CurrentController;
                currentController.FinishTurn();
            }
        }

        public virtual void ProcessSplitButtonClick()
        {
            if (controllerManager.HasActiveController())
            {
                var currentController = controllerManager.CurrentController;
                currentController.OnSplitButtonClick();
            }
        }
    }
}