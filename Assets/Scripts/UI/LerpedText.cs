using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents a patternTextObject, that is scaled in real time.
    /// Text object must be set in the editor.
    /// </summary>
    public class LerpedText : MonoBehaviour
    {
        /// <summary>
        /// Pattern patternTextObject object to display.
        /// </summary>
        [SerializeField] private Text patternTextObject;
        /// <summary>
        /// Current text to display.
        /// </summary>
        private Text currentText;

        /// <summary>
        /// Limit of the scaling (number of times).
        /// </summary>
        [SerializeField] private float maxScale = 3;
        /// <summary>
        /// Works like a speed.
        /// </summary>
        [SerializeField] private float fraction = 1;
        /// <summary>
        /// To compare the current scale with the limit.
        /// </summary>
        private const float eps = 0.05f;

        /// <summary>
        /// An event that is risen when the lerp is finished, i.e. the size reaches the limit.
        /// </summary>
        public event VoidHandler FinishedLerp;

        /// <summary>
        /// Starts displaying the given string, written with the given color.
        /// </summary>
        public void PerformLerpString(string s, Color color)
        {
            //Clone pattern text object and initialize the cloned text.
            currentText = patternTextObject;
            currentText.color = new Color(color.r, color.g, color.b);
            currentText.rectTransform.localScale = new Vector3(1, 1, 1);
            currentText.text = s;
            currentText.enabled = true;
            //Start scaling.
            StartCoroutine(ScaleTextCoroutine());
        }
        
        /// <summary>
        /// Performs the text scaling until it reaches the limit.
        /// </summary>
        private IEnumerator ScaleTextCoroutine()
        {
            while (currentText.rectTransform.localScale.x < maxScale - eps)
            {
                currentText.rectTransform.localScale = Vector3.Lerp(currentText.rectTransform.localScale,
                    new Vector3(maxScale, maxScale, maxScale), Time.deltaTime * fraction);
                yield return null;
            }
            currentText.enabled = false;
            FinishedLerp?.Invoke();
        }
    }
}