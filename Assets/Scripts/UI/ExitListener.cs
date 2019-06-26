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

        /// <summary>
        /// Starts tracking the back button click.
        /// </summary>
        public void Enable()
        {
            StartCoroutine(BackButtonListener());
        }
        
        /// <summary>
        /// Stops tracking the back button click.
        /// </summary>
        public void Disable()
        {
            StopCoroutine(BackButtonListener());
        }

        /// <summary>
        /// Tracks the back button click and raises OnExitClicked event if occurs.
        /// </summary>
        private IEnumerator BackButtonListener()
        {
            while (true)
            {
                //Escape code is equal to the back button on mobile devices.
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnExitClicked?.Invoke();
                }

                yield return null;
            }
        }
    }
}