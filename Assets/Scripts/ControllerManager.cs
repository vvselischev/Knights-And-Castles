namespace Assets.Scripts
{
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

        public bool HasActiveController()
        {
            return CurrentController != null;
        }

        public void DeactivateAll()
        {
            CurrentController = null;
            FirstController.Disable();
            SecondController.Disable();
        }
    }
}