using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Attach it to canvas and it will be automatically activated or deactivated.
    /// </summary>
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