namespace Assets.Scripts
{
    /// <summary>
    /// Simple class for activation/deactivation of different menus in a common way.
    /// </summary>
    public class MenuActivator
    {
        private IMenu currentMenu;
        public static MenuActivator Instance = new MenuActivator();

        private MenuActivator()
        {}

        /// <summary>
        /// Closes the current menu (if exists).
        /// </summary>
        public void CloseMenu()
        {
            currentMenu?.Deactivate();
            currentMenu = null;
        }

        /// <summary>
        /// Activates the given menu.
        /// </summary>
        public void OpenMenu(IMenu newMenu)
        {
            currentMenu = newMenu;
            newMenu?.Activate();
        }
    }
}