using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Simple button. Notifies InputListener, when pressed. Can be locked.
    /// When locked, color is changed to red and clicks are not processed.
    /// </summary>
    public class FinishTurnButton : MonoBehaviour
    {
        [SerializeField] private GameIcon icon;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color lockColor = Color.red;

        public InputListener inputListener { get; set; }

        public void OnClick()
        {
            if (enabled)
            {
                inputListener.ProcessFinishTurnClick();
            }
        }
        
        public void Lock()
        {
            enabled = false;
            icon.ChangeColor(lockColor);
        }

        public void Unlock()
        {
            enabled = true;
            icon.ChangeColor(defaultColor);
        }
    }
}