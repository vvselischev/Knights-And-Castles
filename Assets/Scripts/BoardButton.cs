using UnityEngine;

namespace Assets.Scripts
{
    public class BoardButton : MonoBehaviour
    {
        private int boardX;
        private int boardY;

        [SerializeField] private GameIcon frame;

        public InputListener InputListener { get; set; }

        public void Initialize(int x, int y)
        {
            boardX = x;
            boardY = y;
        }

        public void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessBoardClick(boardX, boardY);
            }
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            DisableFrame();
            enabled = false;
        }
        
        public void DisableFrame()
        {
            frame.Disable();
        }

        public void EnableFrame()
        {
            frame.Enable();
        }
    }
}