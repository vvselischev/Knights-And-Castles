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

        public void SetImage(Image image)
        {
            currentImage.enabled = true;
            currentImage.sprite = image.sprite;
        }

        public Image GetImage()
        {
            return currentImage;
        }

        public void Reset()
        {
            currentImage.sprite = defaultSprite;
        }

        public void ChangeColor(Color color)
        {
            currentImage.color = color;
        }

        /*public void Enable(Image image)
        {
            SetImage(image);
        }*/

        public void Awake()
        {
            defaultSprite = GetComponent<Image>().sprite;
            currentImage = GetComponent<Image>();
        }
    }
}