using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RoundBoardCreator : MonoBehaviour
    {
        public BoardStorage boardStorage;

        //Set in editor
        public Vector2 startFirstPosition = new Vector2(1, 1);
        public Vector2 startSecondPosition = new Vector2(6, 6);

        public GameObject patternIcon;
        public GameObject parent;
        public GameObject patternButton;

        public Sprite NeutralFriendlySprite, NeutralAgressiveSprite, FirstUserSprite, SecondUserSprite;
        
        public Army FirstArmy { get; }
        public Army SecondArmy { get; }

        public void FillBoardStorage()
        {
            System.Random random = new System.Random();
            CheckeredBoard board = boardStorage.board;
            BoardStorageItem[,] storageItems = boardStorage.boardTable;
            for (int col = 1; col <= board.width; col++)
            {
                for (int row = 1; row <= board.height; row++)
                {                    
                    Army currentArmy;
                    Sprite currentSprite;
                    if (new Vector2(col, row) == startFirstPosition)
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST);
                        currentSprite = FirstUserSprite;
                    }
                    else if (new Vector2(col, row) == startSecondPosition)
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND);
                        currentSprite = SecondUserSprite;
                    }
                    else
                    {
                        int randomValue = random.Next() % 3 * 0; //0 -- Empty, 1 -- Friendly, 2 -- Agressive
                        if (randomValue == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (randomValue == 1)
                            {
                                currentSprite = NeutralFriendlySprite;
                                currentArmy = new NeutralFriendlyArmy();
                            }
                            else
                            {
                                currentSprite = NeutralAgressiveSprite;
                                currentArmy = new NeutralAgressiveArmy();
                            }
                        }
                    }

                    GameObject iconGO = InstantiateIcon(currentSprite, col, row);
                    storageItems[col, row] = new ArmyStorageItem(board.BoardButtons[col, row].GetComponent<BoardButton>(),
                        currentArmy, iconGO);
                }
            }
        }

        private GameObject InstantiateIcon(Sprite sprite, int col, int row)
        {
            Image patternImage = patternIcon.GetComponent<Image>();

            Image newImage = Instantiate(patternImage);

            RectTransform rectTransform = newImage.GetComponent<RectTransform>();
            rectTransform.position = patternButton.transform.localPosition +
                                     boardStorage.board.GetOffsetFromPattern(col, row);
            rectTransform.SetParent(parent.transform, false);
            newImage.GetComponent<Image>().sprite = sprite;
            return newImage.gameObject;
        }
    }
}
