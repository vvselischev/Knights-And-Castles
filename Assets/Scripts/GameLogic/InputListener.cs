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
        
        /// <summary>
        /// Method is called when one of the board buttons is clicked.
        /// Receives the button position and transfers it to the current controller if it exists.
        /// </summary>
        public virtual void ProcessBoardClick(int x, int y)
        {
            if (controllerManager.HasActiveController())
            {
                var currentController = controllerManager.CurrentController;
                currentController.OnButtonClick(x, y);
            }
        }

        /// <summary>
        /// Method is called when the finish turn button is clicked.
        /// Notifies the current controller if it exists.
        /// </summary>
        public virtual void ProcessFinishTurnClick()
        {
            if (controllerManager.HasActiveController())
            {
                var currentController = controllerManager.CurrentController;
                currentController.OnFinishTurnClick();
            }
        }

        /// <summary>
        /// Method is called when the split button is clicked.
        /// Notifies the current controller if it exists.
        /// </summary>
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