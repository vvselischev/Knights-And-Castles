using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyMenu : MonoBehaviour, IMenu
    {
        public Text logText;
        public void Activate()
        {
            Debug.Log("Open Lobby Menu");
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            Debug.Log("Close Lobby Menu");
            gameObject.SetActive(false);
        }
    }
}