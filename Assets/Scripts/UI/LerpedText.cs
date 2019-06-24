﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a text, that is scaled in real time.
    /// Text object must be set in the editor.
    /// </summary>
    public class LerpedText : MonoBehaviour
    {
        [SerializeField] public Text text;
        private Text textClone;

        [SerializeField] private float maxScale = 3;
        [SerializeField] private float fraction = 1;
        private const float EPS = 0.05f;

        public event VoidHandler FinishedLerp;

        /// <summary>
        /// Starts displaying the given text, written with the given color.
        /// </summary>
        public void PerformLerpString(string s, Color color)
        {
            textClone = text;
            textClone.color = new Color(color.r, color.g, color.b);
            textClone.rectTransform.localScale = new Vector3(1, 1, 1);
            textClone.text = s;
            textClone.enabled = true;
            StartCoroutine(ScaleTextCoroutine());
        }

        private IEnumerator ScaleTextCoroutine()
        {
            while (textClone.rectTransform.localScale.x < maxScale - EPS)
            {
                textClone.rectTransform.localScale = Vector3.Lerp(textClone.rectTransform.localScale,
                    new Vector3(maxScale, maxScale, maxScale), Time.deltaTime * fraction);
                yield return null;
            }
            textClone.enabled = false;
            FinishedLerp?.Invoke();
        }
    }
}