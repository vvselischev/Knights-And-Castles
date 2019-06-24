using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Rectangle board with buttons on the screen.
    /// </summary>
    [ExecuteInEditMode]
    public class CheckeredButtonBoard : MonoBehaviour
    {
        private GameObject parentObject;
        private Button patternButton;

        public int Width { get; } = 8;
        public int Height { get; } = 10;
        
        private const float spaceBetweenButtons = -2; //experimentally this value is the best
        private static float buttonWidth;
        private static float buttonHeight;

        private Button[,] boardButtons;

        public void Start()
        {
            patternButton = GameObject.Find("PatternButton").GetComponent<Button>();
            parentObject = GameObject.Find("Board");

            buttonWidth = patternButton.GetComponent<RectTransform>().rect.width;
            buttonHeight = patternButton.GetComponent<RectTransform>().rect.height;

            Reset();
        }

        /// <summary>
        /// Recreates the board.
        /// </summary>
        public void Reset()
        {
            DeleteButtons();
            boardButtons = new Button[Width + 1, Height + 1];
            CreateButtons();
        }

        public BoardButton GetBoardButton(IntVector2 position)
        {
            return boardButtons[position.x, position.y].gameObject.GetComponent<BoardButton>();
        }
        
        /// <summary>
        /// Activates all buttons.
        /// </summary>
        public void EnableBoard()
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (var button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().Enable();
            }
        }

        /// <summary>
        /// Deactivates all buttons.
        /// </summary>
        public void DisableBoard()
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (var button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().Disable();
            }
        }

        /// <summary>
        /// Sets an input listener to all buttons.
        /// </summary>
        /// <param name="inputListener"></param>
        public void SetInputListener(InputListener inputListener)
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (var button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().InputListener = inputListener;
            }
        }

        private void DeleteButtons()
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (var button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                DestroyImmediate(button.gameObject);
            }
        }

        private Vector3 GetOffsetFromPattern(int currentColumn, int currentRow)
        {
            return new Vector3((currentColumn - 1) * (buttonWidth + spaceBetweenButtons),
                                                 (currentRow - 1) * (buttonHeight + spaceBetweenButtons));
        }
        
        /// <summary>
        /// Creates the board of buttons -- copies of the patternButton (must be set in the editor).
        /// PatternButton is placed in the bottom-left corner, however, it won't be used later.
        /// </summary>
        private void CreateButtons()
        {
            for (var currentRow = 1; currentRow <= Height; currentRow++)
            {
                for (var currentColumn = 1; currentColumn <= Width; currentColumn++)
                {
                    var offset = GetOffsetFromPattern(currentColumn, currentRow);
                    var newButton = Instantiate(patternButton);
                    var rectTransform = newButton.GetComponent<RectTransform>();

                    //This line seems to be useless (it doesn't change size)
                    rectTransform.rect.size.Set(buttonWidth, buttonHeight);

                    rectTransform.position = patternButton.transform.localPosition + offset;
                    rectTransform.SetParent(parentObject.transform, false);

                    newButton.gameObject.SetActive(true);
                    newButton = InitButton(newButton, currentColumn, currentRow);

                    boardButtons[currentColumn, currentRow] = newButton;
                }
            }
        }

        private Button InitButton(Button newButton, int x, int y)
        {
            var boardButton = newButton.GetComponent<BoardButton>();
            boardButton.Initialize(x, y);
            return boardButton.GetComponent<Button>();
        }
    }
}