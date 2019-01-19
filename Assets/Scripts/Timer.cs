using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.Scripts;

public class Timer : MonoBehaviour
{
    public Text timeText;
    public int duration = 30;

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
                yield break;
            }
            yield return null;
        }
    }

    public void StopTimer()
    {
        Reset();
        started = false;
        StopCoroutine(UpdateTimer());
    }

    public void Reset()
    {
        //secondsLeft = 0;
        //timeText.text = "0";
    }

    public void AddTime(int deltaTime)
    {
        secondsLeft += deltaTime;
    }
}