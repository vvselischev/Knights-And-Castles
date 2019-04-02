using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class ExitListener : MonoBehaviour
{
    public event VoidHandler OnExitClicked;

    public void Enable()
    {
        StartCoroutine(BackButtonListener());
    }

    public void Disable()
    {
        StopCoroutine(BackButtonListener());
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
    
    protected IEnumerator BackButtonListener()
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