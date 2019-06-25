using System.Collections.Generic;
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

        /// <summary>
        /// Width of the board.
        /// </summary>
        public int Width { get; } = 8;
        /// <summary>
        /// Height of the board.
        /// </summary>
        public int Height { get; } = 10;
        
        //Strange value, without it adjacent buttons collapse.
        //Experimentally this value is the best.
        private const float spaceBetweenButtons = -2; 
        private static float buttonWidth;
        private static float buttonHeight;
        
        /// <summary>
        /// An array of buttons.
        /// </summary>
        private Button[,] boardButtons;
        /// <summary>
        /// A list of buttons wrapped to board button object to simply enumerate them.
        /// </summary>
        private List<BoardButton> buttonsList = new List<BoardButton>();

        /// <summary>
        /// Initializes pattern button.
        /// Recreates the board.
        /// </summary>
        public void Start()
        {
            //Search the scene for the objects by their names.
            patternButton = GameObject.Find("PatternButton").GetComponent<Button>();
            parentObject = GameObject.Find("Board");

            buttonWidth = patternButton.GetComponent<RectTransform>().rect.width;
            buttonHeight = patternButton.GetComponent<RectTransform>().rect.height;
            
            //Recreate the board.
            Reset();
        }

        /// <summary>
        /// Recreates the board.
        /// </summary>
        public void Reset()
        {
            DeleteButtons();
            boardButtons = new Button[Width + 1, Height + 1];
            buttonsList = new List<BoardButton>();
            CreateButtons();
        }

        /// <summary>
        /// Returns the button by the given position.
        /// </summary>
        public BoardButton GetBoardButton(IntVector2 position)
        {
            return boardButtons[position.x, position.y].gameObject.GetComponent<BoardButton>();
        }

        public IEnumerable<BoardButton> GetBoardButtons()
        {
            return buttonsList;
        }
        
        /// <summary>
        /// Activates all buttons.
        /// </summary>
        public void EnableBoard()
        {
            foreach (var button in buttonsList)
            {
                button.gameObject.GetComponent<BoardButton>().Enable();
            }
        }

        /// <summary>
        /// Deactivates all buttons.
        /// </summary>
        public void DisableBoard()
        {
            foreach (var button in buttonsList)
            {
                button.gameObject.GetComponent<BoardButton>().Disable();
            }
        }

        /// <summary>
        /// Sets the given input listener to all buttons.
        /// </summary>
        public void SetInputListener(InputListener inputListener)
        {
            foreach (var button in buttonsList)
            {
                button.gameObject.GetComponent<BoardButton>().InputListener = inputListener;
            }
        }

        /// <summary>
        /// Destroys all buttons.
        /// </summary>
        private void DeleteButtons()
        {
            foreach (var button in buttonsList)
            {
                DestroyImmediate(button.gameObject);
            }
        }

        /// <summary>
        /// Returns the distance in both dimensions from pattern button object to the given position.
        /// </summary>
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
                    //Clone the pattern button and place it to the current position.
                    var offset = GetOffsetFromPattern(currentColumn, currentRow);
                    var newButton = Instantiate(patternButton);
                    var rectTransform = newButton.GetComponent<RectTransform>();

                    rectTransform.position = patternButton.transform.localPosition + offset;
                    rectTransform.SetParent(parentObject.transform, false);

                    //Initialize the button.
                    newButton.gameObject.SetActive(true);
                    newButton = InitButton(newButton, currentColumn, currentRow);

                    //Remember the button.
                    boardButtons[currentColumn, currentRow] = newButton;
                    buttonsList.Add(newButton.GetComponent<BoardButton>());
                }
            }
        }

        /// <summary>
        /// Initializes the button with the given coordinates.
        /// </summary>
        private Button InitButton(Button newButton, int x, int y)
        {
            var boardButton = newButton.GetComponent<BoardButton>();
            boardButton.Initialize(x, y);
            return boardButton.GetComponent<Button>();
        }
    }
}