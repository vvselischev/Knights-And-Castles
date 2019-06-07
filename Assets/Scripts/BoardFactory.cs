﻿using System;
using System.Collections.Generic;
 using System.Linq;
 using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BoardFactory : MonoBehaviour
    {
        private System.Random random = new System.Random();

        private BoardConfiguration configuration;

        private int blockWidth;
        private int blockHeight;
        private int blocksHorizontal;
        private int blocksVertical;
        
        [SerializeField] private GameObject patternIcon;
        [SerializeField] private GameObject parent;

        [SerializeField] private CheckeredButtonBoard board; //just to transfer it to board storage 
        [SerializeField] private BoardManager boardManager; //just to transfer ift to passes

        private int imbalance = startImbalance; // first is always in better position
        private const int startImbalance = 100;
        private const int RandomNumberOfUnitsFrom = 10;
        private const int RandomNumberOfUnitsTo = 30;
        

        [SerializeField] private Sprite neutralFriendlySprite;

        [SerializeField] private Sprite neutralAggressiveSprite;

        [SerializeField] private Sprite firstUserSprite;

        [SerializeField] private Sprite secondUserSprite;

        [SerializeField] private Sprite castleSprite;

        [SerializeField] private Sprite passSprite;

        public BlockBoardStorage CreateEmptyStorage(BoardType configurationType, out BoardManager boardManager)
        {
            if (configurationType == BoardType.SMALL)
            {
                configuration = new SmallBoardConfiguration();
            }
            else
            {
                configuration = new LargeBoardConfiguration();
            }
            imbalance = startImbalance;
            blockWidth = configuration.BlockWidth;
            blockHeight = configuration.BlockHeight;
            blocksHorizontal = configuration.BlocksHorizontal;
            blocksVertical = configuration.BlocksVertical;
            var storage = new BlockBoardStorage(blocksHorizontal, blocksVertical, board);
            boardManager = new BoardManager(storage, configuration.FirstStartBlock, configuration.SecondStartBlock);
            this.boardManager = boardManager;
            return storage;
        }
        
        public void FillBoardStorageRandomly(IBoardStorage boardStorage)
        {
            var currentBoardTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            var currentBonusTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            
            InstantiateCastles(currentBonusTable);
            InstantiatePasses(currentBonusTable);
            
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {                    
                    Army currentArmy;
                    Sprite currentSprite;
                    if (ExistsPlayerArmy(col, row, PlayerType.FIRST))
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST, GenerateArmyComposition(RandomNumberOfUnitsFrom, 
                            RandomNumberOfUnitsTo));
                        currentSprite = firstUserSprite;
                    }
                    else if (ExistsPlayerArmy(col, row, PlayerType.SECOND))
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, GenerateArmyComposition(RandomNumberOfUnitsFrom, 
                            RandomNumberOfUnitsTo));
                        currentSprite = secondUserSprite;
                    }
                    else if (ExistsPass(col, row))
                    {
                        //We do not want to have a 'surprise' army at the end of the pass.
                        continue;
                    }
                    else
                    {
                        //0 -- Empty, 1 -- Friendly, 2, 3 -- Aggressive
                        var randomValue = random.Next() % 4;
                        if (randomValue == 0)
                        {
                            continue;
                        }

                        if (randomValue == 1)
                        {
                            currentSprite = neutralFriendlySprite;
                            currentArmy = new NeutralFriendlyArmy(
                                GenerateBalancedArmyComposition(true, new IntVector2(col, row)));
                        }
                        else
                        {
                            currentSprite = neutralAggressiveSprite;
                            currentArmy = new NeutralAggressiveArmy(
                                GenerateBalancedArmyComposition(false, new IntVector2(col, row)));
                        }
                    }

                    var iconGO = InstantiateIcon(currentSprite);
                    iconGO.SetActive(false);
                    currentBoardTable[col, row] = new ArmyStorageItem(currentArmy, iconGO);
                }
            }
            
            boardStorage.Fill(currentBoardTable, currentBonusTable);
        }

        private bool ExistsPlayerArmy(int col, int row, PlayerType playerType)
        {
            var position = new IntVector2(col, row);
            if (playerType == PlayerType.FIRST)
            {
                var startFirstPositions = configuration.StartFirstPositions;
                return startFirstPositions.Any(localPosition =>
                    GetGlobalPosition(localPosition, configuration.FirstStartBlock).Equals(position));
            }

            if (playerType == PlayerType.SECOND)
            {
                var startSecondPositions = configuration.StartSecondPositions;
                if (startSecondPositions.Any(localPosition =>
                    GetGlobalPosition(localPosition, configuration.SecondStartBlock).Equals(position)))
                {
                    return true;
                }    
            }
            return false;
        }

        private bool ExistsPass(int col, int row)
        {
            var position = new IntVector2(col, row);
            for (var i = 0; i < configuration.PassesNumber; i++)
            {
                var globalFromPosition = GetGlobalPosition(configuration.PassesFromPositions[i],
                    configuration.PassesFromBlocks[i]);
                var globalToPosition = GetGlobalPosition(configuration.PassesToPositions[i],
                    configuration.PassesToBlocks[i]);
                if (position.Equals(globalFromPosition) || position.Equals(globalToPosition))
                {
                    return true;
                }
            }
            return false;
        }

        private IntVector2 GetGlobalPosition(IntVector2 localPosition, IntVector2 block)
        {
            var globalX = (block.x - 1) * blockWidth + localPosition.x;
            var globalY = (block.y - 1) * blockHeight + localPosition.y;  
            return new IntVector2(globalX, globalY);
        }
        
        private void InstantiatePasses(BoardStorageItem[,] bonusTable)
        {
            for (var i = 0; i < configuration.PassesNumber; i++)
            {
                var passObject = InstantiateIcon(passSprite);
                passObject.SetActive(false);
                var pass = new Pass(passObject, boardManager, configuration.PassesToBlocks[i], 
                    configuration.PassesFromPositions[i], configuration.PassesToPositions[i]);

                var globalFrom = GetGlobalPosition(configuration.PassesFromPositions[i], configuration.PassesFromBlocks[i]);
                bonusTable[globalFrom.x, globalFrom.y] = pass;
            }
        }

        private void InstantiateCastles(BoardStorageItem[,] bonusTable)
        {
            InstantiateCastlesFromList(configuration.FirstCastlesPositions, configuration.FirstCastlesBlocks,
                PlayerType.FIRST, bonusTable);
            InstantiateCastlesFromList(configuration.SecondCastlesPositions, configuration.SecondCastlesBlocks, 
                PlayerType.SECOND, bonusTable);
        }

        private void InstantiateCastlesFromList(IntVector2[] positions, IntVector2[] blocks, 
            PlayerType ownerType, BoardStorageItem[,] bonusTable)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                var castleObject = InstantiateIcon(castleSprite);
                castleObject.SetActive(false);
                var castle = new Castle(castleObject, ownerType);
                var globalPosition = GetGlobalPosition(positions[i], blocks[i]);
                bonusTable[globalPosition.x, globalPosition.y] = castle;
            }
        }


        //From left to right, from bottom to up.
        //x == 0 => Empty
        //x == 1 => NeutralFriendly
        //x == 2 => NeutralAggressive
        //x == 11 => FirstPlayer
        //x == 12 => SecondPlayer
        //x = 1, 2, 11, 12 => after x goes (v1, v2, v3) -- spearmen, archers, cavalrymen
        public void FillBoardStorageFromArray(byte[] array, IBoardStorage boardStorage)
        {            
            var currentBoardTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            var currentBonusTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            
            InstantiateCastles(currentBonusTable);
            InstantiatePasses(currentBonusTable);
            
            var currentInd = 0;
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {
                    var currentType = array[currentInd];
                    currentInd++;

                    if (currentType == 0)
                    {
                        continue;
                    }

                    Army currentArmy = null;
                    Sprite currentSprite = null;
                    
                    var spearmen = array[currentInd];
                    currentInd++;
                    var archers = array[currentInd];
                    currentInd++;
                    var cavalrymen = array[currentInd];
                    currentInd++;
                    var armyComposition = new ArmyComposition(spearmen, archers, cavalrymen);

                    if (currentType == 11)
                    {
                        currentArmy = new UserArmy(PlayerType.FIRST, armyComposition);
                        currentSprite = firstUserSprite;
                    }
                    else if (currentType == 12)
                    {
                        currentArmy = new UserArmy(PlayerType.SECOND, armyComposition);
                        currentSprite = secondUserSprite;
                    }
                    else if (currentType == 1)
                    {
                        currentArmy = new NeutralFriendlyArmy(armyComposition);
                        currentSprite = neutralFriendlySprite;
                    }
                    else if (currentType == 2)
                    {
                        currentArmy = new NeutralAggressiveArmy(armyComposition);
                        currentSprite = neutralAggressiveSprite;
                    }

                    var iconGO = InstantiateIcon(currentSprite);
                    iconGO.SetActive(false);
                    currentBoardTable[col, row] = new ArmyStorageItem(currentArmy, iconGO);
                }
            }
            
            boardStorage.Fill(currentBoardTable, currentBonusTable);
        }

        public List<byte> ConvertBoardStorageToBytes(BlockBoardStorage boardStorage)
        {
            boardStorage.ConvertToArrays(out var items, out _);
            
            var byteList = new List<byte>();
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {
                    byte currentType = 0;
                    Army army = null;
                    if (items[col, row] != null && items[col, row] is ArmyStorageItem)
                    {
                        army = ((ArmyStorageItem) items[col, row]).Army;
                        if (army.PlayerType == PlayerType.FIRST)
                        {
                            currentType = 11;
                        }
                        else if (army.PlayerType == PlayerType.SECOND)
                        {
                            currentType = 12;
                        }
                        else if (army.PlayerType == PlayerType.NEUTRAL)
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
                        var armyComposition = army.ArmyComposition;
                        byteList.Add((byte)armyComposition.Spearmen);
                        byteList.Add((byte)armyComposition.Archers);
                        byteList.Add((byte)armyComposition.Cavalrymen);
                    }
                }
            }

            return byteList;
        }

        public GameObject CloneBoardIcon(IBoardStorage boardStorage, int fromX, int fromY)
        {
            var item = boardStorage.GetItem(fromX, fromY);
            return InstantiateIcon(item.StoredObject.GetComponent<Image>().sprite);
        }
        
        private GameObject InstantiateIcon(Sprite sprite)
        {
            var patternImage = patternIcon.GetComponent<Image>();

            var newImage = Instantiate(patternImage);
            newImage.enabled = true;

            var rectTransform = newImage.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.transform, false);
            newImage.GetComponent<Image>().sprite = sprite;
            
            return newImage.gameObject;
        }

        private ArmyComposition GenerateArmyComposition(int from, int to)
        {
            int randomSpearmen = (from + random.Next() % (to - from)) % 100,
                randomArchers = (from + random.Next() % (to - from)) % 100,
                randomCavalrymen = (from + random.Next() % (to - from)) % 100;
            return new ArmyComposition(randomSpearmen, randomArchers, randomCavalrymen);
        }

        /// <summary>
        /// Field imbalance stores current imbalance of current game state.
        /// Analyzing this information it can be added more or less units to the generated army.
        /// For these purposes in this function multiplier is calculated which generated army composition multiplies on.
        /// </summary>
        private ArmyComposition GenerateBalancedArmyComposition(bool isFriendly, IntVector2 position)
        {
            var boardWidthPlusHeight = blockWidth * blocksHorizontal + blockHeight * blocksVertical;
            var balancePositionMultiplier = boardWidthPlusHeight - 2 * (position.x + position.y);
            if (!isFriendly)
            {
                balancePositionMultiplier *= -1;
            }

            var additional = Math.Abs(balancePositionMultiplier / (double)boardWidthPlusHeight);

            double multiplier;
            if (HasSameSign(imbalance, balancePositionMultiplier))
            {
                multiplier = 1 - additional + 0.05; // to avoid zero division
            }
            else
            {
                multiplier = 1 + additional + 0.05; // to avoid zero division
            }

            var resultArmyComposition = GenerateArmyComposition(
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