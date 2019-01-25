﻿using System;
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
        private System.Random random = new System.Random();

        // TODO: ArmyComposition generation
        //Set in editor

        public Vector2[] startFirstPositions = { new Vector2(1, 1), new Vector2(1, 2) };
        public Vector2[] startSecondPositions = { new Vector2(6, 6), new Vector2(5, 6) };

        public GameObject patternIcon;
        public GameObject parent;
        public GameObject patternButton;

        public Sprite NeutralFriendlySprite, NeutralAgressiveSprite, FirstUserSprite, SecondUserSprite;
        
        public Army FirstArmy { get; }
        public Army SecondArmy { get; }

        public void FillBoardStorage()
        {
            CheckeredButtonBoard board = boardStorage.board;
            BoardStorageItem[,] storageItems = boardStorage.boardTable;
            for (int col = 1; col <= board.width; col++)
            {
                for (int row = 1; row <= board.height; row++)
                {                    
                    Army currentArmy;
                    Sprite currentSprite;
                    if (Array.Exists(startFirstPositions, position => position == new Vector2(col, row)))
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST, GenerateArmyComposition());
                        currentSprite = FirstUserSprite;
                    }
                    else if (Array.Exists(startSecondPositions, position => position == new Vector2(col, row)))
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, GenerateArmyComposition());
                        currentSprite = SecondUserSprite;
                    }
                    else
                    {
                        int randomValue = random.Next() % 3; //0 -- Empty, 1 -- Friendly, 2 -- Agressive
                        if (randomValue == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (randomValue == 1)
                            {
                                currentSprite = NeutralFriendlySprite;
                                currentArmy = new NeutralFriendlyArmy(GenerateArmyComposition());
                            }
                            else
                            {
                                currentSprite = NeutralAgressiveSprite;
                                currentArmy = new NeutralAgressiveArmy(GenerateArmyComposition());
                            }
                        }
                    }

                    GameObject iconGO = InstantiateIcon(currentSprite, col, row);
                    storageItems[col, row] = new ArmyStorageItem(currentArmy, iconGO);
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

        private ArmyComposition GenerateArmyComposition()
        {
            int randomMice = random.Next() % 1000,
                randomCats = random.Next() % 1000,
                randomElephants = random.Next() % 1000;
            return new ArmyComposition(randomMice, randomCats, randomElephants);
        }
    }
}
