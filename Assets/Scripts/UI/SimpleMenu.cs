using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Attach it to canvas and it will be automatically activated or deactivated.
    /// </summary>
    public class SimpleMenu : MonoBehaviour, IMenu
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}