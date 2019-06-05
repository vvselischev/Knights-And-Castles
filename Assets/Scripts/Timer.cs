using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.Scripts;

public class Timer : MonoBehaviour
{
    [SerializeField] protected Text timeText;
    [SerializeField] protected int duration = 30;

    public event VoidHandler OnFinish;

    private long secondsLeft;
    private bool started;
    private long timeInStart;

    public void StartTimer()
    {
        secondsLeft = duration;
        started = true;
        timeInStart = GetCurrentTimeScaled();
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (started)
        {
            var timeNow = GetCurrentTimeScaled();
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

    private long GetCurrentTimeScaled()
    {
        return DateTime.Now.ToBinary() / 10000000;
    }
    
    public void StopTimer()
    {
        started = false;
        StopCoroutine(UpdateTimer());       
    }
}