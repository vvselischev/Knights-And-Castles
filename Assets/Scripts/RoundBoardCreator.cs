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
        public Vector2[] startSecondPositions = { new Vector2(8, 10), new Vector2(8, 9) };

        public GameObject patternIcon;
        public GameObject parent;
        public GameObject patternButton;

        private int imbalance = 1000; // first is always in better position
        private const int randomNumberOfUnitsFrom = 500;
        private const int randomNumberOfUnitsTo = 1000;

        public Sprite NeutralFriendlySprite, NeutralAgressiveSprite, FirstUserSprite, SecondUserSprite;
        
        public Army FirstArmy { get; }
        public Army SecondArmy { get; }

        public void FillBoardStorageRandomly()
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
                        currentArmy = new UserArmy(PlayerType.FIRST, GenerateArmyComposition(randomNumberOfUnitsFrom, randomNumberOfUnitsTo));
                        currentSprite = FirstUserSprite;
                    }
                    else if (Array.Exists(startSecondPositions, position => position == new Vector2(col, row)))
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, GenerateArmyComposition(randomNumberOfUnitsFrom, randomNumberOfUnitsTo));
                        currentSprite = SecondUserSprite;
                    }
                    else
                    {
                        int randomValue = random.Next() % 3; //0 -- Empty, 1 -- Friendly, 2 -- Agressive
                        if (randomValue == 0)
                        {
                            continue;
                        }

                        if (randomValue == 1)
                        {
                            currentSprite = NeutralFriendlySprite;
                            currentArmy = new NeutralFriendlyArmy(
                                GenerateBalancedArmyComposition(true, new Vector2(col, row)));
                        }
                        else
                        {
                            currentSprite = NeutralAgressiveSprite;
                            currentArmy = new NeutralAggressiveArmy(
                                GenerateBalancedArmyComposition(false, new Vector2(col, row)));
                        }
                    }

                    GameObject iconGO = InstantiateIcon(currentSprite, col, row);
                    storageItems[col, row] = new ArmyStorageItem(currentArmy, iconGO);
                }
            }
            Debug.Log("current imbalance:" + imbalance);
        }

        //From left to right, from bottom to up.
        //x == 0 => Empty
        //x == 1 => NeutralFriendly
        //x == 2 => NeutralAggressive
        //x == 11 => FirstPlayer
        //x == 12 => SecondPlayer
        //x > 10 => after x go (v1, v2, v3) -- spearmen, archers, cavalrymen
        public void FillBoardStorageFromArray(byte[] array)
        {
            CheckeredButtonBoard board = boardStorage.board;
            BoardStorageItem[,] storageItems = boardStorage.boardTable;
            int currentInd = 0;
            for (int row = 1; row <= board.height; row++)
            {
                for (int col = 1; col <= board.width; col++)
                {
                    byte currentType = array[currentInd];
                    currentInd++;

                    if (currentType == 0)
                    {
                        continue;
                    }

                    Army currentArmy = null;
                    Sprite currentSprite = null;
                    
                    byte spearmen = array[currentInd];
                    currentInd++;
                    byte archers = array[currentInd];
                    currentInd++;
                    byte cavalrymen = array[currentInd];
                    currentInd++;
                    ArmyComposition armyComposition = new ArmyComposition(spearmen, archers, cavalrymen);

                    if (currentType == 11)
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST, armyComposition);
                        currentSprite = SecondUserSprite;
                    }
                    else if (currentType == 12)
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, armyComposition);
                        currentSprite = FirstUserSprite;
                    }
                    else if (currentType == 1)
                    {
                        currentArmy = new NeutralFriendlyArmy(armyComposition);
                        currentSprite = NeutralFriendlySprite;
                    }
                    else if (currentType == 2)
                    {
                        currentArmy = new NeutralAggressiveArmy(armyComposition);
                        currentSprite = NeutralAgressiveSprite;
                    }

                    GameObject iconGO = InstantiateIcon(currentSprite, col, row);
                    storageItems[col, row] = new ArmyStorageItem(currentArmy, iconGO);
                }
            }
        }

        public List<byte> ConvertBoardStorageToBytes()
        {
            CheckeredButtonBoard board = boardStorage.board;
            BoardStorageItem[,] storageItems = boardStorage.boardTable;
            List<byte> byteList = new List<byte>();
            for (int row = 1; row <= board.height; row++)
            {
                for (int col = 1; col <= board.width; col++)
                {
                    byte currentType = 0;
                    Army army = null;
                    if (storageItems[col, row] is ArmyStorageItem)
                    {
                        army = (storageItems[col, row] as ArmyStorageItem).Army;
                        if (army.playerType == PlayerType.FIRST)
                        {
                            currentType = 11;
                        }
                        else if (army.playerType == PlayerType.SECOND)
                        {
                            currentType = 12;
                        }
                        else if (army.playerType == PlayerType.NEUTRAL)
                        {
                            if (army is NeutralFriendlyArmy)
                            {
                                currentType = 1;
                            }
                            else if (army is NeutralAggressiveArmy)
                            {
                                currentType = 2;
                            }
                        }
                    }
                    byteList.Add(currentType);
                    
                    if (currentType != 0)
                    {
                        ArmyComposition armyComposition = army.armyComposition;
                        byteList.Add((byte)armyComposition.spearmen);
                        byteList.Add((byte)armyComposition.archers);
                        byteList.Add((byte)armyComposition.cavalrymen);
                    }
                }
            }

            return byteList;
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

        private ArmyComposition GenerateArmyComposition(int from, int to)
        {
            int randomMice = (from + random.Next() % (to - from)) % 100,
                randomCats = (from + random.Next() % (to - from)) % 100,
                randomElephants = (from + random.Next() % (to - from)) % 100;
            return new ArmyComposition(randomMice, randomCats, randomElephants);
        }

        /*
         Field imbalance stores current imbalance of current game state.
         Analyzing this information it can be added more or less units to the generated army.
         For these purposes in this function calculates multiplier on which generated army composition multiplies.
         */
        private ArmyComposition GenerateBalancedArmyComposition(bool isFriendly, Vector2 position)
        {
            int boardWidthPlusHeight = boardStorage.board.width + boardStorage.board.height;
            int balancePositionMultiplier = boardWidthPlusHeight - 2 * (int)(position.x + position.y);
            if (!isFriendly)
            {
                balancePositionMultiplier *= -1;
            }

            double additional = Math.Abs(balancePositionMultiplier / (double)boardWidthPlusHeight);

            double multiplier = 0;
            if (HasSameSign(imbalance, balancePositionMultiplier))
            {
                multiplier = 1 - additional + 0.05; // to avoid zero division
            }
            else
            {
                multiplier = 1 + additional + 0.05; // to avoid zero division
            }

            Debug.Log("GenerateBalancedArmyComposition");

            ArmyComposition resultArmyComposition = GenerateArmyComposition(
                (int)(randomNumberOfUnitsFrom * multiplier), (int)(randomNumberOfUnitsTo * multiplier));

            if (imbalance < 0)
            {
                imbalance += resultArmyComposition.TotalUnitQuantity();
            }
            else
            {
                imbalance -= resultArmyComposition.TotalUnitQuantity();
            }

            return resultArmyComposition;
        }

        private bool HasSameSign(int a, int b)
        {
            return Math.Sign(a) == Math.Sign(b);
        }
    }
}
