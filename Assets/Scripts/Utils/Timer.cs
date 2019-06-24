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
        [SerializeField] protected Text timeText;
        [SerializeField] protected int duration = 30;

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