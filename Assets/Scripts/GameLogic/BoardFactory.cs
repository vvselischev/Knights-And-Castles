using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Class for board storage creation, serialization and deserialization.
    /// When creating storage randomly, fair algorithm is used to make players' parts of board
    /// have approximately the same amount of enemy and friendly units.
    /// Board has 1-indexation.
    /// </summary>
    public class BoardFactory : MonoBehaviour
    {
        private System.Random random = new System.Random();

        /// <summary>
        /// Temporary variables to share configuration between board creation methods.
        /// </summary>
        private BoardConfiguration configuration;
        private int blockWidth;
        private int blockHeight;
        private int blocksHorizontal;
        private int blocksVertical;
        
        /// <summary>
        /// Game object of pattern icons of armies on board.
        /// </summary>
        [SerializeField] private GameObject patternIcon;
        /// <summary>
        /// Relative parent for army icons on board.
        /// </summary>
        [SerializeField] private GameObject parent;

        [SerializeField] private CheckeredButtonBoard board; //just to transfer it to board storage 
        [SerializeField] private BoardManager boardManager; //just to transfer it to passes

        /// <summary>
        /// The difference in player's forces.
        /// </summary>
        private int imbalance = startImbalance; // first is always in better position
        private const int startImbalance = 100;
        
        /// <summary>
        /// Minimum number of units during the creation.
        /// </summary>
        private const int randomNumberOfUnitsFrom = 10;
        /// <summary>
        /// Maximum number of units during the creation.
        /// </summary>
        private const int randomNumberOfUnitsTo = 30;

        /// <summary>
        /// Probabilities for the particular cell during random filling.
        /// Their sum must be equal to 1.
        /// </summary>
        private const double emptyCellProbability = 0.25;
        private const double neutralFriendlyCellProbability = 0.25;
        private const double neutralAggressiveCellProbability = 0.5;
        

        /// <summary>
        /// Sprites for icons on board.
        /// </summary>
        [SerializeField] private Sprite neutralFriendlySprite;
        [SerializeField] private Sprite neutralAggressiveSprite;
        [SerializeField] private Sprite firstUserSprite;
        [SerializeField] private Sprite secondUserSprite;
        [SerializeField] private Sprite firstUserCastleSprite;
        [SerializeField] private Sprite secondUserCastleSprite;
        [SerializeField] private Sprite passSprite;

        /// <summary>
        /// Integer ids of different army types for de-/serialization and random choice.
        /// </summary>
        private const int emptyCellId = 0;
        private const int neutralFriendlyCellId = 1;
        private const int neutralAggressiveCellId = 2;
        // Here is the gap for other possible neutral types.
        private const int firstPlayerCellId = 11;
        private const int secondPlayerCellId = 12;

        /// <summary>
        /// Creates an empty block storage of the given type and boardManager associated with it.
        /// </summary>
        public BlockBoardStorage CreateEmptyBlockStorage(BoardType configurationType, out BoardManager boardManager)
        {
            var configuration = GetConfigurationByType(configurationType);
            var storage = new BlockBoardStorage(configuration.BlocksHorizontal, configuration.BlocksHorizontal, board);
            boardManager = new BoardManager(storage, configuration.FirstStartBlock, configuration.SecondStartBlock);
            return storage;
        }

        /// <summary>
        /// Returns the object with board configuration by the given type.
        /// </summary>
        private BoardConfiguration GetConfigurationByType(BoardType configurationType)
        {
            if (configurationType == BoardType.SMALL)
            {
                return configuration = new SmallBoardConfiguration();
            }
            return configuration = new LargeBoardConfiguration();
        }
        
        /// <summary>
        /// Creates tables of army and bonus layers for the whole board.
        /// Tables are 1-indexed.
        /// </summary>
        private void CreateGlobalTables(out BoardStorageItem[,] boardTable, out BoardStorageItem[,] bonusTable)
        {
            boardTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
            bonusTable = new BoardStorageItem[blockWidth * blocksHorizontal + 1, blockHeight * blocksVertical + 1];
        }

        /// <summary>
        /// Fills the given storage randomly according to the given configuration type.
        /// The given board manager must be the one produced in constructor when creating given board storage.
        /// It will be transferred to passes.
        /// Creates two tables for army and bonus layers, fills them with BoardStorageItems (Army + Icon) and
        /// transfers them to the board storage.
        /// Note, that this method works properly with any implementations of IBoardStorage.
        /// </summary>
        public void FillBoardStorageRandomly(IBoardStorage boardStorage, BoardType configurationType, 
            BoardManager boardManager)
        {
            //Initialize and save most common parameters to fields in order not to touch the configuration object every time.
            InitializeConfigurationParameters(configurationType, boardManager);
            
            //Create tables for army and bonus layers.
            CreateGlobalTables(out var currentBoardTable, out var currentBonusTable);
            
            //Fill bonus layer.
            FillBonusTable(currentBonusTable);

            //Fill army layer.
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {
                    if (!InitializeCell(col, row, out var currentArmy, out var currentSprite))
                    {
                        //The cell is empty.
                        continue;
                    }
                    //Now we know that the cell is not empty.
                    CompleteArmyCellInitialization(currentBoardTable, col, row, currentArmy, currentSprite);
                }
            }
            //Fill the storage using filled tables.
            boardStorage.Fill(currentBoardTable, currentBonusTable);
        }

        /// <summary>
        /// Fills the bonus layer of the board, namely castles and passes.
        /// </summary>
        private void FillBonusTable(BoardStorageItem[,] bonusTable)
        {
            InstantiateCastles(bonusTable);
            InstantiatePasses(bonusTable);
        }

        /// <summary>
        /// Save most common parameters to fields so as not to touch the configuration object every time (for convenience)
        /// The same is with the board manager to transfer it to passes.
        /// Initializes the imbalance for army generation.
        /// </summary>
        private void InitializeConfigurationParameters(BoardType configurationType, BoardManager boardManager)
        {
            configuration = GetConfigurationByType(configurationType);
            blockWidth = configuration.BlockWidth;
            blockHeight = configuration.BlockHeight;
            blocksHorizontal = configuration.BlocksHorizontal;
            blocksVertical = configuration.BlocksVertical;
            this.boardManager = boardManager;
            //Initialize also imbalance.
            imbalance = startImbalance;
        }

        /// <summary>
        /// Determines whether the given cell will have an army (user or neutral) or it will be empty.
        /// In the first case produces not null generated army and corresponding sprite, and returns true.
        /// In the second case outer parameters are null and method returns false.
        /// </summary>
        private bool InitializeCell(int col, int row, out Army currentArmy, out Sprite currentSprite)
        {
            currentArmy = null;
            currentSprite = null;
            if (ExistsPlayerArmy(col, row, PlayerType.FIRST))
            {
                InitializePlayerArmyCell(PlayerType.FIRST, out currentArmy, out currentSprite);
            }
            else if (ExistsPlayerArmy(col, row, PlayerType.SECOND))
            {
                InitializePlayerArmyCell(PlayerType.SECOND, out currentArmy, out currentSprite);
            }
            else if (ExistsPass(col, row))
            {
                //We do not want to have a 'surprise' army at the end of the pass.
                return false;
            }
            else if (ExistsCastle(col, row))
            {
                //And we do not to have neutrals on castles.
                return false;
            }
            else
            {
                var cellTypeId = GetRandomCellTypeId();
                if (cellTypeId == emptyCellId)
                {
                    return false;
                }

                InitializeNeutralCell(cellTypeId, col, row, out currentArmy, out currentSprite);    
            }
            return true;
        }

        /// <summary>
        /// Generates an army for player and produces the corresponding sprite by the given player type.
        /// </summary>
        private void InitializePlayerArmyCell(PlayerType playerType, out Army army, out Sprite sprite)
        {
            sprite = null;
            army = new UserArmy(playerType, GenerateArmyComposition(randomNumberOfUnitsFrom, randomNumberOfUnitsTo));
            if (playerType == PlayerType.FIRST)
            {
                sprite = firstUserSprite;
            }
            else if (playerType == PlayerType.SECOND)
            {
                sprite = secondUserSprite;
            }
        }

        /// <summary>
        /// Creates an icon by the given sprite (it is inactive at this point) and fills the given cell with
        /// created ArmyStorageItem.
        /// </summary>
        private void CompleteArmyCellInitialization(BoardStorageItem[,] currentBoardTable, int col, int row,
            Army currentArmy, Sprite currentSprite)
        {
            var iconGO = InstantiateIcon(currentSprite);
            iconGO.SetActive(false);
            currentBoardTable[col, row] = new ArmyStorageItem(currentArmy, iconGO);
        }

        /// <summary>
        /// Produces an army and corresponding sprite by the given neutral type.
        /// 
        /// </summary>
        private void InitializeNeutralCell(int neutralTypeId, int col, int row, out Army army, out Sprite sprite)
        {
            army = null;
            sprite = null;
            if (neutralTypeId == neutralFriendlyCellId)
            {
                sprite = neutralFriendlySprite;
                army = new NeutralFriendlyArmy(GenerateBalancedArmyComposition(ArmyType.NEUTRAL_FRIENDLY, 
                    new IntVector2(col, row)));
            }
            else if (neutralTypeId == neutralAggressiveCellId)
            {
                sprite = neutralAggressiveSprite;
                army = new NeutralAggressiveArmy(GenerateBalancedArmyComposition(ArmyType.NEUTRAL_AGGRESSIVE, 
                    new IntVector2(col, row)));
            }
        }

        /// <summary>
        /// Returns the cell id with the probabilities described as constants above.
        /// </summary>
        private int GetRandomCellTypeId()
        {
            var randomValue = random.NextDouble();
            if (randomValue < emptyCellProbability)
            {
                return emptyCellId;
            }
            if (emptyCellProbability <= randomValue && 
                randomValue < emptyCellProbability + neutralFriendlyCellProbability)
            {
                return neutralFriendlyCellId;
            }
            return neutralAggressiveCellId;
        }

        /// <summary>
        /// Determines whether the given cell contains a given player army according to the configuration.
        /// </summary>
        private bool ExistsPlayerArmy(int col, int row, PlayerType playerType)
        {
            var position = new IntVector2(col, row);
            //We are just creating the board, so it is enough to look at start player positions in the configuration.
            if (playerType == PlayerType.FIRST)
            {
                var startFirstPositions = configuration.StartFirstPositions;
                //Configurations store position as local to the corresponding blocks, so we must convert it to global.
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

        /// <summary>
        /// Determines whether the given cell contains a pass entrance or a pass exit according to the configuration.
        /// </summary>
        private bool ExistsPass(int col, int row)
        {
            var position = new IntVector2(col, row);
            for (var i = 0; i < configuration.PassesNumber; i++)
            {
                //Given coordinates are global, so we need to convert the position in the configuration to global.
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
        
        /// <summary>
        /// Determines whether the given cell contains a castle according to the configuration.
        /// </summary>
        private bool ExistsCastle(int col, int row)
        {
            var position = new IntVector2(col, row);
            for (var i = 0; i < configuration.FirstCastlesBlocks.Length; i++)
            {
                //Given coordinates are global, so we need to convert the position in the configuration to global.
                var globalPosition = GetGlobalPosition(configuration.FirstCastlesPositions[i],
                    configuration.FirstCastlesBlocks[i]);
                if (position.Equals(globalPosition))
                {
                    return true;
                }
            }
            
            for (var i = 0; i < configuration.SecondCastlesBlocks.Length; i++)
            {
                //Given coordinates are global, so we need to convert the position in the configuration to global.
                var globalPosition = GetGlobalPosition(configuration.SecondCastlesPositions[i],
                    configuration.SecondCastlesBlocks[i]);
                if (position.Equals(globalPosition))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the position on the whole board by the position inside the block and the position of the block.
        /// </summary>
        private IntVector2 GetGlobalPosition(IntVector2 localPosition, IntVector2 block)
        {
            //Positions are 1-indexed!
            var globalX = (block.x - 1) * blockWidth + localPosition.x;
            var globalY = (block.y - 1) * blockHeight + localPosition.y;  
            return new IntVector2(globalX, globalY);
        }
        
        /// <summary>
        /// Fills the given table with passes according to the configuration.
        /// Only the entrance of the pass is set to the storage, since we do not care where it leads.
        /// </summary>
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

        /// <summary>
        /// Fills the given table with castles of both players according to the configuration.
        /// </summary>
        private void InstantiateCastles(BoardStorageItem[,] bonusTable)
        {
            InstantiateCastlesForPlayer(configuration.FirstCastlesPositions, configuration.FirstCastlesBlocks,
                PlayerType.FIRST, bonusTable);
            InstantiateCastlesForPlayer(configuration.SecondCastlesPositions, configuration.SecondCastlesBlocks, 
                PlayerType.SECOND, bonusTable);
        }
        
        /// <summary>
        /// Fills the table with castles of the given player by given arrays of position and corresponding blocks.
        /// </summary>
        private void InstantiateCastlesForPlayer(IntVector2[] positions, IntVector2[] blocks, 
            PlayerType ownerType, BoardStorageItem[,] bonusTable)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                Sprite castleSprite = null;
                if (ownerType == PlayerType.FIRST)
                {
                    castleSprite = firstUserCastleSprite;
                }
                else if (ownerType == PlayerType.SECOND)
                {
                    castleSprite = secondUserCastleSprite;
                }
                var castleObject = InstantiateIcon(castleSprite);
                castleObject.SetActive(false);
                var castle = new Castle(castleObject, ownerType);
                var globalPosition = GetGlobalPosition(positions[i], blocks[i]);
                bonusTable[globalPosition.x, globalPosition.y] = castle;
            }
        }

        /// <summary>
        /// Converts the given array of bytes to the board storage by the following convention:
        /// From left to right, from bottom to up.
        /// First the cellTypeId goes.
        /// If it is not an id of an empty cell, it is followed by three bytes: spearmen, archers, cavalrymen
        /// in the current army respectively.
        /// The rest of the initialization process is the same as in the random filling.
        /// </summary>
        public void FillBoardStorageFromArray(byte[] array, IBoardStorage boardStorage, BoardType configurationType, 
            BoardManager boardManager)
        {
            InitializeConfigurationParameters(configurationType, boardManager);
            CreateGlobalTables(out var currentBoardTable, out var currentBonusTable);
            FillBonusTable(currentBonusTable);
            
            var currentInd = 0;
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {
                    var currentType = array[currentInd];
                    currentInd++;

                    if (currentType == emptyCellId)
                    {
                        continue;
                    }

                    var spearmen = array[currentInd];
                    currentInd++;
                    var archers = array[currentInd];
                    currentInd++;
                    var cavalrymen = array[currentInd];
                    currentInd++;
                    var armyComposition = new ArmyComposition(spearmen, archers, cavalrymen);

                    InitializeCellById(currentType, armyComposition, out var currentArmy, out var currentSprite);
                    CompleteArmyCellInitialization(currentBoardTable, col, row, currentArmy, currentSprite);
                }
            }
            boardStorage.Fill(currentBoardTable, currentBonusTable);
        }

        /// <summary>
        /// Produces an army with the given composition and a corresponding sprite by the given cellTypeId.
        /// </summary>
        private void InitializeCellById(byte cellTypeId, ArmyComposition armyComposition, 
            out Army army, out Sprite sprite)
        {
            army = null;
            sprite = null;
            if (cellTypeId == firstPlayerCellId)
            {
                army = new UserArmy(PlayerType.FIRST, armyComposition);
                sprite = firstUserSprite;
            }
            else if (cellTypeId == secondPlayerCellId)
            {
                army = new UserArmy(PlayerType.SECOND, armyComposition);
                sprite = secondUserSprite;
            }
            else if (cellTypeId == neutralFriendlyCellId)
            {
                army = new NeutralFriendlyArmy(armyComposition);
                sprite = neutralFriendlySprite;
            }
            else if (cellTypeId == neutralAggressiveCellId)
            {
                army = new NeutralAggressiveArmy(armyComposition);
                sprite = neutralAggressiveSprite;
            }
        }

        /// <summary>
        /// Converts the given storage to the list of bytes by the same convention as in FillBoardStorageFromArray.
        /// Bonus layer is stored in the configuration so we do not need to transfer them.
        /// </summary>
        public List<byte> ConvertBoardStorageToBytes(BlockBoardStorage boardStorage)
        {
            //Ignore bonus layer.
            boardStorage.ConvertToArrays(out var items, out _);
            
            var byteList = new List<byte>();
            for (var col = 1; col <= blockWidth * blocksHorizontal; col++)
            {
                for (var row = 1; row <= blockHeight * blocksVertical; row++)
                {
                    byte currentType = emptyCellId;
                    Army army = null;
                    if (items[col, row] != null && items[col, row] is ArmyStorageItem)
                    {
                        army = ((ArmyStorageItem) items[col, row]).Army;
                        currentType = (byte)GetArmyIdType(army);
                    }
                    byteList.Add(currentType);
                    
                    if (currentType != emptyCellId) // it is an army, not an empty cell
                    {
                        SerializeArmyComposition(army, byteList);
                    }
                }
            }

            return byteList;
        }

        /// <summary>
        /// Adds an army composition to the given list.
        /// Sequentially the number of spearmen, archers and cavalrymen is added (in this order, converted to byte).
        /// </summary>
        private void SerializeArmyComposition(Army army, List<byte> bytes)
        {
            var armyComposition = army.ArmyComposition;
            bytes.Add((byte) armyComposition.Spearmen);
            bytes.Add((byte) armyComposition.Archers);
            bytes.Add((byte) armyComposition.Cavalrymen);
        }

        /// <summary>
        /// Returns the cellTypeId of the given army.
        /// </summary>
        private int GetArmyIdType(Army army)
        {
            var currentType = emptyCellId;
            if (army.PlayerType == PlayerType.FIRST)
            {
                currentType = firstPlayerCellId;
            }
            else if (army.PlayerType == PlayerType.SECOND)
            {
                currentType = secondPlayerCellId;
            }
            else if (army.PlayerType == PlayerType.NEUTRAL)
            {
                if (army is NeutralFriendlyArmy)
                {
                    currentType = neutralFriendlyCellId;
                }
                else if (army is NeutralAggressiveArmy)
                {
                    currentType = neutralAggressiveCellId;
                }
            }
            return currentType;
        }

        /// <summary>
        /// Returns a copy of boardIcon on the given position of the storage.
        /// Icon is placed on the same position.
        /// </summary>
        public GameObject CloneBoardIcon(IBoardStorage boardStorage, int fromX, int fromY)
        {
            var item = boardStorage.GetItem(fromX, fromY);
            return InstantiateIcon(item.StoredObject.GetComponent<Image>().sprite);
        }
        
        /// <summary>
        /// Creates a copy of the pattern icon with the given sprite.
        /// Parent transform is set in the editor.
        /// The copy is placed on the position of the pattern icon (do not rely on it).
        /// Return the game object of the copied icon.
        /// </summary>
        private GameObject InstantiateIcon(Sprite sprite)
        {
            //Take pattern image and clone it.
            var patternImage = patternIcon.GetComponent<Image>();
            var newImage = Instantiate(patternImage);
            newImage.enabled = true;

            //Set the parent transform to the copied image and the given sprite.
            var rectTransform = newImage.GetComponent<RectTransform>();
            rectTransform.SetParent(parent.transform, false);
            newImage.GetComponent<Image>().sprite = sprite;
            
            //Return its game object.
            return newImage.gameObject;
        }

        /// <summary>
        /// Generates a random army composition where units are in the given diapason
        /// but not less than randomNumberOfUnitsFrom and not greater than randomNumberOfUnitsTo.
        /// </summary>
        private ArmyComposition GenerateArmyComposition(int from, int to)
        {
            var randomSpearmen = GenerateRandomNumberOfUnits(from, to);
            var randomArchers = GenerateRandomNumberOfUnits(from, to);
            var randomCavalrymen = GenerateRandomNumberOfUnits(from, to);
            return new ArmyComposition(randomSpearmen, randomArchers, randomCavalrymen);
        }

        /// <summary>
        /// Generates a random number of units in the given diapason in the given diapason
        /// but not less than randomNumberOfUnitsFrom and not greater than randomNumberOfUnitsTo.
        /// </summary>
        private int GenerateRandomNumberOfUnits(int from, int to)
        {
            var result = from + random.Next() % (to - from);
            result = Math.Max(randomNumberOfUnitsFrom, result);
            result = Math.Min(result, randomNumberOfUnitsTo);
            return result;
        }

        /// <summary>
        /// Field imbalance stores current imbalance of current game state.
        /// Analyzing this information it can be added more or less units to the generated army.
        /// For these purposes in this function multiplier is calculated which generated army composition multiplies on.
        /// </summary>
        private ArmyComposition GenerateBalancedArmyComposition(ArmyType armyType, IntVector2 position)
        {
            var boardWidthPlusHeight = blockWidth * blocksHorizontal + blockHeight * blocksVertical;
            var balancePositionMultiplier = boardWidthPlusHeight - 2 * (position.x + position.y);
            if (armyType != ArmyType.NEUTRAL_FRIENDLY)
            {
                balancePositionMultiplier *= -1;
            }

            var additional = Math.Abs(balancePositionMultiplier / (double)boardWidthPlusHeight);
            var multiplier = GetMultiplier(balancePositionMultiplier, additional);

            var resultArmyComposition = GenerateArmyComposition((int)(randomNumberOfUnitsFrom * multiplier), 
                (int)(randomNumberOfUnitsTo * multiplier));

            UpdateImbalance(resultArmyComposition);
            return resultArmyComposition;
        }

        private void UpdateImbalance(ArmyComposition resultArmyComposition)
        {
            if (imbalance < 0)
            {
                imbalance += resultArmyComposition.TotalUnitQuantity();
            }
            else
            {
                imbalance -= resultArmyComposition.TotalUnitQuantity();
            }
        }

        private double GetMultiplier(int balancePositionMultiplier, double additional)
        {
            // to avoid zero division
            const double zeroOffset = 0.05;
            double multiplier;
            if (HasSameSign(imbalance, balancePositionMultiplier))
            {
                multiplier = 1 - additional + zeroOffset; 
            }
            else
            {
                multiplier = 1 + additional + zeroOffset;
            }
            return multiplier;
        }

        private bool HasSameSign(int a, int b)
        {
            return Math.Sign(a) == Math.Sign(b);
        }
    }
}