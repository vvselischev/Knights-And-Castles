using Assets.Scripts;
using UnityEngine;

public class StartGameState : MonoBehaviour, IGameState
{
    private MenuActivator menuActivator = MenuActivator.GetInstance();
    public StartMenu startMenu;

    public void InvokeState()
    {
        menuActivator.OpenMenu(startMenu);
    }

    public void CloseState()
    {
        menuActivator.CloseMenu();
    }
}