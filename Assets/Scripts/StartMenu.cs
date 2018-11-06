using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    //в этих методах прописываем конкретные кнопки, надписи и т.д., вешаем на Canvas
    public class StartMenu : MonoBehaviour, IMenu
    {
        public Canvas canvas;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            Debug.Log("Closing Start Menu");
            gameObject.SetActive(false);
        }
    }
}