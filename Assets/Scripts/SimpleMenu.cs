using UnityEngine;

namespace Assets.Scripts
{
    public class SimpleMenu : MonoBehaviour, IMenu
    {
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}