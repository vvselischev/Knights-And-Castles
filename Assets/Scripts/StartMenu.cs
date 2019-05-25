using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class StartMenu : MonoBehaviour, IMenu
    {
        public void Activate()
        {
            Debug.Log("Open Start Menu");
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            Debug.Log("Close Start Menu");
            gameObject.SetActive(false);
        }
    }
}