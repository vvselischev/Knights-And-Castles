using UnityEngine;

namespace Assets.Scripts
{
    //TODO: make it and start button children of common class
    public class SplitButton : MonoBehaviour
    {
        [SerializeField] private GameIcon icon;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color lockColor = Color.red;

        public InputListener InputListener { get; set; }

        public void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessSplitButtonClick();
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