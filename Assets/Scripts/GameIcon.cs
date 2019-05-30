using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameIcon : MonoBehaviour
    {
        private Sprite defaultSprite;
        private Image currentImage;

        public bool enabledByDefault = true;

        public void Reset()
        {
            currentImage.enabled = enabledByDefault;
            currentImage.sprite = defaultSprite;
        }

        public void ChangeColor(Color color)
        {
            currentImage.enabled = true;
            currentImage.color = color;
        }

        public void Disable()
        {
            Reset();
            currentImage.enabled = false;
        }
        
        public void Enable()
        {
            currentImage.enabled = true;
            currentImage.color = Color.yellow;
        }

        public void Awake()
        {
            defaultSprite = GetComponent<Image>().sprite;
            currentImage = GetComponent<Image>();
            enabledByDefault = currentImage.enabled;
        }
    }
}