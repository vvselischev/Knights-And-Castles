using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public Text text;
        private Text textClone;

        public float maxScale = 3;
        public float fraction = 1;
        private const float EPS = 0.05f;
        public Canvas PlayCanvas;

        public event VoidHandler FinishedLerp;

        void Start()
        {
            PlayCanvas.enabled = false;
        }

        public void PerformLerpString(string s, Color color)
        {
            textClone = text;
            textClone.color = new Color(color.r, color.g, color.b);
            textClone.rectTransform.localScale = new Vector3(1, 1, 1);
            textClone.text = s;
            textClone.enabled = true;
            StartCoroutine(ScaleTextCoroutine());
        }

        IEnumerator ScaleTextCoroutine()
        {
            while (textClone.rectTransform.localScale.x < maxScale - EPS) //TODO: fix .x
            {
                textClone.rectTransform.localScale = Vector3.Lerp(textClone.rectTransform.localScale,
                    new Vector3(maxScale, maxScale, maxScale), Time.deltaTime * fraction);
                yield return null;
            }
            Debug.Log("Lerp finished");
            textClone.enabled = false;
            FinishedLerp?.Invoke();
        }

        public void DisableText()
        {
            StopCoroutine(ScaleTextCoroutine());
            textClone.enabled = false;
        }
    }
}