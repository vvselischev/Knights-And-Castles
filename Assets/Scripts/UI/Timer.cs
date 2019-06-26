using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Simple custom timer, updating associated text every second.
    /// Raises OnFinish event when finishes.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Text to display the remaining time.
        /// </summary>
        [SerializeField] private Text timeText;
        /// <summary>
        /// The duration.
        /// </summary>
        [SerializeField] private int duration = 30;

        /// <summary>
        /// This event is risen when the timer finishes.
        /// </summary>
        public event VoidHandler OnFinish;

        private long secondsLeft;
        private bool started;
        private long timeInStart;

        /// <summary>
        /// Starts the timer. Duration now is set in the editor.
        /// </summary>
        public void StartTimer()
        {
            secondsLeft = duration;
            started = true;
            timeInStart = GetCurrentTimeSeconds();
            StartCoroutine(UpdateTimer());
        }

        /// <summary>
        /// Updates the timer and the text.
        /// </summary>
        private IEnumerator UpdateTimer()
        {
            while (started)
            {
                var timeNow = GetCurrentTimeSeconds();
                timeText.text = (secondsLeft + timeInStart - timeNow).ToString();
                if (secondsLeft + timeInStart - timeNow <= 0)
                {
                    StopTimer();
                    OnFinish?.Invoke();
                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Returns the current time in seconds.
        /// </summary>
        private long GetCurrentTimeSeconds()
        {
            return (long)TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds; 
        }

        /// <summary>
        /// Stops the timer immediately.
        /// </summary>
        public void StopTimer()
        {
            started = false;
            StopCoroutine(UpdateTimer());
        }
    }
}