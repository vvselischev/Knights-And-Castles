namespace Assets.Scripts
{
    /// <summary>
    /// Class for convenient change between players' controllers.
    /// </summary>
    public class ControllerManager
    {
        public UserController CurrentController { get; private set; }
        public UserController FirstController { get; }
        public UserController SecondController { get; }

        public ControllerManager(UserController firstController, UserController secondController)
        {
            FirstController = firstController;
            SecondController = secondController;
        }
        
        /// <summary>
        /// Sets the current controller, but does not activate it.
        /// </summary>
        public void SetCurrentController(TurnType type)
        {
            if (type == TurnType.SECOND)
            {
                CurrentController = SecondController;
            }
            else if (type == TurnType.FIRST)
            {
                CurrentController = FirstController;
            }
        }

        /// <summary>
        /// Enables the controller associated with the given TurnType.
        /// </summary>
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

        /// <summary>
        /// Disables the controller associated with the given TurnType.
        /// </summary>
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

        /// <summary>
        /// Returns true if there exists active controller and false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool HasActiveController()
        {
            return CurrentController != null;
        }

        /// <summary>
        /// Deactivates both controllers.
        /// </summary>
        public void DeactivateAll()
        {
            CurrentController = null;
            FirstController.Disable();
            SecondController.Disable();
        }
    }
}