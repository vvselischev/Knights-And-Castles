using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    //простой класс для включения/выключения разных меню
    public class MenuActivator
    {
        private IMenu currentMenu;
        private static MenuActivator instance = new MenuActivator();

        public static MenuActivator GetInstance()
        {
            return instance;
        }

        private MenuActivator()
        {
        }

        public void ChangeMenu(IMenu newMenu)
        {
            if (currentMenu != null)
            {
                CloseMenu();
            }
            currentMenu = newMenu;
            newMenu.Activate();
        }

        public void CloseMenu()
        {
            //Debug.Log("Current Menu: " + currentMenu);
            currentMenu?.Deactivate();
            currentMenu = null;
        }

        public void OpenMenu(IMenu newMenu)
        {
            currentMenu = newMenu;
            newMenu.Activate();
        }
    }
}