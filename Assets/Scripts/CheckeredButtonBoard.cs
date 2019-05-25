using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class CheckeredButtonBoard : MonoBehaviour
    {
        private GameObject parentObject;
        private Button patternButton;

        public int width = 8;
        public int height = 10;
        private const float SPACE_BETWEEN_BUTTONS = -2; //-2.44f; //buttonWidth/20;

        private static float ButtonWidth;
        private static float ButtonHeight;
        

        private Button[,] boardButtons;

        public void Start()
        {
            patternButton = GameObject.Find("PatternButton").GetComponent<Button>();
            parentObject = GameObject.Find("Board");

            ButtonWidth = patternButton.GetComponent<RectTransform>().rect.width;
            ButtonHeight = patternButton.GetComponent<RectTransform>().rect.height;

            Reset();
        }

        public void Reset()
        {
            DeleteButtons();
            boardButtons = new Button[width + 1, height + 1];
            CreateButtons();
        }

        public BoardButton GetBoardButton(IntVector2 position)
        {
            return boardButtons[position.x, position.y].gameObject.GetComponent<BoardButton>();
        }
        
        public void EnableBoard()
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (Button button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().Enable();
            }
        }

        public void DisableBoard()
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (Button button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().Disable();
            }
        }

        public void SetInputListener(InputListener inputListener)
        {
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (Button button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                button.gameObject.GetComponent<BoardButton>().inputListener = inputListener;
            }
        }

        private void DeleteButtons()
        {
            Debug.Log("Deleting buttons");
            var buttons = FindObjectsOfType(typeof(Button));
            foreach (Button button in buttons.Cast<Button>().Where(button => button.gameObject.name.Contains("Clone")))
            {
                DestroyImmediate(button.gameObject);
            }
        }

        private Vector3 GetOffsetFromPattern(int currentColumn, int currentRow)
        {
            return new Vector3((currentColumn - 1) * (ButtonWidth + SPACE_BETWEEN_BUTTONS),
                                                 (currentRow - 1) * (ButtonHeight + SPACE_BETWEEN_BUTTONS));
        }

        //PatternButton is placed in the bottom-left corner.
        private void CreateButtons()
        {
            Debug.Log("Creating buttons");
            for (int currentRow = 1; currentRow <= height; currentRow++)
            {
                for (int currentColumn = 1; currentColumn <= width; currentColumn++)
                {
                    Vector3 offset = GetOffsetFromPattern(currentColumn, currentRow);
                    Button newButton = Instantiate(patternButton);
                    RectTransform rectTransform = newButton.GetComponent<RectTransform>();

                    //This line seems to be useless (it doesn't change size)
                    rectTransform.rect.size.Set(ButtonWidth, ButtonHeight);

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
            BoardButton boardButton = newButton.GetComponent<BoardButton>();
            boardButton.Initialize(x, y);
            return boardButton.GetComponent<Button>();
        }
    }
}