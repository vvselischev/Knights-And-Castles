﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoardFactory : MonoBehaviour
    {
        private BlockBoardStorage boardStorage;
        private BoardStorageItem[,] currentBoardTable;
        private BoardStorageItem[,] currentBonusTable;
        private System.Random random = new System.Random();

        //Set in editor
        public int blocksHorizontal = 1;
        public int blocksVertical = 1;

        public int blockWidth = 8;
        public int blockHeight = 10;
        
        public IntVector2[] startFirstPositions = { new IntVector2(1, 1), new IntVector2(1, 2) };
        public IntVector2[] startSecondPositions = { new IntVector2(8, 10), new IntVector2(8, 9) };
        
        public IntVector2[] firstCastlesPositions = { new IntVector2(1, 1) };
        public IntVector2[] secondCastlesPositions = { new IntVector2(8, 10) };

        public const int PASSES_NUMBER = 2;

        public IntVector2[] passesFromBlocks = new IntVector2[PASSES_NUMBER]
        {
            new IntVector2(1, 1),
            new IntVector2(1, 2) 
        };
        
        public IntVector2[] passesToBlocks = new IntVector2[PASSES_NUMBER]
        {
            new IntVector2(1, 2),
            new IntVector2(1, 1)
        };
        
        public IntVector2[] passesFromPositions = new IntVector2[PASSES_NUMBER]
        {
            new IntVector2(2, 1),
            new IntVector2(4, 5) 
        };
        
        //It will be more convenient for user to be placed next to the pass, but not on the same cell.
        public IntVector2[] passesToPositions = new IntVector2[PASSES_NUMBER]
        {
            new IntVector2(4, 6),
            new IntVector2(4, 4) 
        };

        public GameObject patternIcon;
        public GameObject parent;
        public GameObject patternButton;

        public CheckeredButtonBoard board; //just to transfer it to board storage 
        public BoardManager boardManager;

        private int imbalance = startImbalance; // first is always in better position
        private const int startImbalance = 100;
        private const int RandomNumberOfUnitsFrom = 500;
        private const int RandomNumberOfUnitsTo = 1000;

        public Sprite NeutralFriendlySprite, NeutralAgressiveSprite, FirstUserSprite, SecondUserSprite, CastleSprite, PassSprite;

        public BlockBoardStorage Initialize()
        {
            boardStorage = new BlockBoardStorage(blocksHorizontal, blocksVertical, board);
            
            imbalance = startImbalance;
            return boardStorage;
        }
        
        public void FillBoardStorageRandomly()
        {
            currentBoardTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            currentBonusTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            
            InstantiateCastles();
            InstantiatePasses();
            
            for (int col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (int row = 1; row <= blockHeight * blocksVertical; row++)
                {                    
                    Army currentArmy;
                    Sprite currentSprite;
                    if (Array.Exists(startFirstPositions, position => position.Equals(new IntVector2(col, row))))
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST, GenerateArmyComposition(RandomNumberOfUnitsFrom, RandomNumberOfUnitsTo));
                        currentSprite = FirstUserSprite;
                    }
                    else if (Array.Exists(startSecondPositions, position => position.Equals(new IntVector2(col, row))))
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, GenerateArmyComposition(RandomNumberOfUnitsFrom, RandomNumberOfUnitsTo));
                        currentSprite = SecondUserSprite;
                    }
                    else if (ExistsPass(col, row))
                    {
                        //We do not want to have a 'surprise' army at the end of the pass.
                        continue;
                    }
                    else
                    {
                        int randomValue = random.Next() % 3; //0 -- Empty, 1 -- Friendly, 2 -- Aggressive
                        if (randomValue == 0)
                        {
                            continue;
                        }

                        if (randomValue == 1)
                        {
                            currentSprite = NeutralFriendlySprite;
                            currentArmy = new NeutralFriendlyArmy(
                                GenerateBalancedArmyComposition(true, new IntVector2(col, row)));
                        }
                        else
                        {
                            currentSprite = NeutralAgressiveSprite;
                            currentArmy = new NeutralAggressiveArmy(
                                GenerateBalancedArmyComposition(false, new IntVector2(col, row)));
                        }
                    }

                    GameObject iconGO = InstantiateIcon(currentSprite);
                    iconGO.SetActive(false);
                    currentBoardTable[col, row] = new ArmyStorageItem(currentArmy, iconGO);
                }
            }
            Debug.Log("current imbalance:" + imbalance);
            
            boardStorage.Fill(currentBoardTable, currentBonusTable);
        }

        private bool ExistsPass(int col, int row)
        {
            var position = new IntVector2(col, row);
            for (int i = 0; i < PASSES_NUMBER; i++)
            {
                var globalFromPosition = GetGlobalPosition(passesFromPositions[i], passesFromBlocks[i]);
                var globalToPosition = GetGlobalPosition(passesToPositions[i], passesToBlocks[i]);
                if (position.Equals(globalFromPosition) || position.Equals(globalToPosition))
                {
                    return true;
                }
            }
            return false;
        }

        private IntVector2 GetGlobalPosition(IntVector2 localPosition, IntVector2 block)
        {
            int globalX = (block.x - 1) * blockWidth + localPosition.x;
            int globalY = (block.y - 1) * blockHeight + localPosition.y;  
            return new IntVector2(globalX, globalY);
        }
        
        private void InstantiatePasses()
        {
            for (int i = 0; i < PASSES_NUMBER; i++)
            {
                var passObject = InstantiateIcon(PassSprite);
                passObject.SetActive(false);
                var pass = new Pass(passObject, boardManager, passesToBlocks[i], 
                    passesFromPositions[i], passesToPositions[i]);

                var globalFrom = GetGlobalPosition(passesFromPositions[i], passesFromBlocks[i]);
                currentBonusTable[globalFrom.x, globalFrom.y] = pass;
            }
        }

        private void InstantiateCastles()
        {
            InstantiateCastlesFromList(firstCastlesPositions, PlayerType.FIRST);
            InstantiateCastlesFromList(secondCastlesPositions, PlayerType.SECOND);
        }

        private void InstantiateCastlesFromList(IntVector2[] positions, PlayerType ownerType)
        {
            foreach (var position in positions)
            {
                var castleObject = InstantiateIcon(CastleSprite);
                castleObject.SetActive(false);
                var castle = new Castle(castleObject) {ownerType = ownerType};
                currentBonusTable[position.x, position.y] = castle;
            }
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
            InstantiateCastles();
            
            int currentInd = 0;
            for (int row = 1; row <= blockHeight * blocksVertical; row++)
            {
                for (int col = 1; col <= blockWidth * blocksHorizontal; col++)
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
                        currentSprite = FirstUserSprite;
                    }
                    else if (currentType == 12)
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, armyComposition);
                        currentSprite = SecondUserSprite;
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

                    GameObject iconGO = InstantiateIcon(currentSprite);
                    
                    //TODO!!!
                    boardStorage.SetItem(col, row, new ArmyStorageItem(currentArmy, iconGO));
                }
            }
        }

        public List<byte> ConvertBoardStorageToBytes()
        {
            List<byte> byteList = new List<byte>();
            for (int row = 1; row <= boardStorage.GetBoardHeight(); row++)
            {
                for (int col = 1; col <= boardStorage.GetBoardWidth(); col++)
                {
                    byte currentType = 0;
                    Army army = null;
                    if (boardStorage.GetItem(col, row) is ArmyStorageItem)
                    {
                        army = (boardStorage.GetItem(col, row) as ArmyStorageItem).Army;
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

        public GameObject CloneBoardIcon(int fromX, int fromY)
        {
            BoardStorageItem item = boardStorage.GetItem(fromX, fromY);
            return InstantiateIcon(item.StoredObject.GetComponent<Image>().sprite);
        }
        
        private GameObject InstantiateIcon(Sprite sprite)
        {
            Image patternImage = patternIcon.GetComponent<Image>();

            Image newImage = Instantiate(patternImage);
            newImage.enabled = true;

            RectTransform rectTransform = newImage.GetComponent<RectTransform>();
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
         For these purposes in this function multiplier is calculated which generated army composition multiplies on.
         */
        private ArmyComposition GenerateBalancedArmyComposition(bool isFriendly, IntVector2 position)
        {
            int boardWidthPlusHeight = blockWidth + blockHeight;
            int balancePositionMultiplier = boardWidthPlusHeight - 2 * (position.x + position.y);
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
                (int)(RandomNumberOfUnitsFrom * multiplier), (int)(RandomNumberOfUnitsTo * multiplier));

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