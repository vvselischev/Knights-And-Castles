using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    /// <summary>
    /// Tracks the back button (escape on keyboard) input when enabled.
    /// </summary>
    public class ExitListener : MonoBehaviour
    {
        /// <summary>
        /// Rises this event when back button is pressed.
        /// </summary>
        public event VoidHandler OnExitClicked;

        public void Enable()
        {
            StartCoroutine(BackButtonListener());
        }

        public void Disable()
        {
            StopCoroutine(BackButtonListener());
        }

        private IEnumerator BackButtonListener()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnExitClicked?.Invoke();
                }

                yield return null;
            }
        }
    }
}