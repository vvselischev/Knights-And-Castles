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
        timeInStart = DateTime.Now.ToBinary() / 10000000;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (started)
        {
            timeText.text = (secondsLeft + timeInStart - DateTime.Now.ToBinary() / 10000000).ToString();
            if (secondsLeft + timeInStart - DateTime.Now.ToBinary() / 10000000 <= 0)
            {
                StopTimer();
                OnFinish?.Invoke();
                yield break;
            }
            yield return null;
        }
    }

    public void StopTimer()
    {
        started = false;
        StopCoroutine(UpdateTimer());       
    }
}