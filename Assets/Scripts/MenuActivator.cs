namespace Assets.Scripts
{
    //простой класс для включения/выключения разных меню
    public class MenuActivator
    {
        private IMenu currentMenu;
        public static MenuActivator Instance = new MenuActivator();

        private MenuActivator()
        {}

        public void CloseMenu()
        {
            currentMenu?.Deactivate();
            currentMenu = null;
        }

        public void OpenMenu(IMenu newMenu)
        {
            currentMenu = newMenu;
            newMenu?.Activate();
        }
    }
}