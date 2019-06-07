using System;

namespace Assets.Scripts
{
    //TODO: null -> empty object

    /// <summary>
    /// Performs game logic after button click.
    /// </summary>
    public class UserController
    {
        /// <summary>
        /// Stores event handler on finish move
        /// </summary>
        public event VoidHandler FinishedMove;
        private BoardFactory boardFactory;
        private PlayGameState playGameState;
        private BlockBoardStorage boardStorage;
        private PlayerType playerType;
        private ArmyStorageItem chosenArmyItem;
        private ArmyText armyText;
        
        private IntVector2 chosenArmyPosition;
        private IntVector2 activeFramePosition;
        private ObjectMover currentMover;
        private IntVector2 currentTargetPosition;
        private bool splitButtonClicked;

        private const int MAX_ARMIES = 5;

        public UserController(PlayerType playerType, BlockBoardStorage storage, BoardFactory boardFactory, 
            PlayGameState playGameState, ArmyText armyText)
        {
            this.playerType = playerType;
            boardStorage = storage;
            this.boardFactory = boardFactory;
            this.playGameState = playGameState;
            this.armyText = armyText;
        }

        /// <summary>
        /// Processes right action after click on board cell with given coordinates  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OnButtonClick(int x, int y)
        {
            ProcessAction(new IntVector2(x, y));
        }

        private void ProcessAction(IntVector2 position)
        {
            var chooseOrMoveClick = false; 
            
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                if (ProcessClickOnSameCell(position))
                {
                    return;
                }
                
                SetActiveFrame(position);
                var clickedArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
                armyText.UpdateText(clickedArmyItem.Army);

                chooseOrMoveClick = ProcessChooseClick(position, clickedArmyItem);
            }
            else
            {
                if (ProcessSplitClickOnAdjacent(position))
                {
                    return;
                }
            }

            if (ProcessMoveClickOnAdjacent(position))
            {
                return;
            }
            
            if (!chooseOrMoveClick)
            {
                chosenArmyItem = null;
                if (boardStorage.GetBonusItem(position) is Pass)
                {
                    var pass = boardStorage.GetBonusItem(position) as Pass;
                    pass.ChangeBlock();
                }    
            }
            splitButtonClicked = false;
        }

        private bool ProcessChooseClick(IntVector2 position, ArmyStorageItem clickedArmyItem)
        {
            if (clickedArmyItem.Army.PlayerType != playerType || !((UserArmy) clickedArmyItem.Army).IsActive())
            {
                return false;
            }

            if (chosenArmyItem != null && ReachableFromChosen(position))
            {
                return true;
            }
            
            chosenArmyItem = clickedArmyItem;
            chosenArmyPosition = position;
            return true;
        }

        private bool ProcessMoveClickOnAdjacent(IntVector2 position)
        {
            if ((chosenArmyItem == null) || !ReachableFromChosen(position) ||
                !(chosenArmyItem.Army as UserArmy).IsActive() || splitButtonClicked)
            {
                return false;
            }
            
            (chosenArmyItem.Army as UserArmy).SetInactive();
            MoveChosen(position);
            splitButtonClicked = false;
            return true;

        }

        private bool ProcessSplitClickOnAdjacent(IntVector2 position)
        {
            armyText.Clear();
            SetActiveFrame(null);
            
            if (chosenArmyItem == null || boardStorage.GetBonusItem(position) is Pass)
            {
                return false;
            }

            if (!ReachableFromChosen(position) || !splitButtonClicked ||
                boardStorage.FindPlayerArmies(playerType).Count >= MAX_ARMIES)
            {
                return false;
            }
            
            ProcessSplit(chosenArmyPosition.x, chosenArmyPosition.y, position.x, position.y);
            SetActiveFrame(position);
            splitButtonClicked = false;
            chosenArmyItem = null;
            return true;

        }

        private bool ProcessClickOnSameCell(IntVector2 position)
        {
            if (activeFramePosition == null || !activeFramePosition.Equals(position))
            {
                return false;
            }
            
            SetActiveFrame(null);
            chosenArmyItem = null;
            chosenArmyPosition = null;
            splitButtonClicked = false;
            armyText.Clear();
            return true;

        }

        private void SetActiveFrame(IntVector2 position)
        {
            if (activeFramePosition != null)
            {
                boardStorage.DisableFrame(activeFramePosition);
            }

            if (position != null)
            {
                boardStorage.EnableFrame(position);
            }

            activeFramePosition = position;
        }
 

        private void ProcessSplit(int chosenPositionX, int chosenPositionY, int positionX, int positionY)
        {
            var splittedArmyPart = chosenArmyItem.Army.SplitIntoEqualParts();
            var clonedIcon = boardFactory.CloneBoardIcon(boardStorage, chosenPositionX, chosenPositionY);
            boardStorage.SetItem(positionX, positionY, new ArmyStorageItem(splittedArmyPart, clonedIcon));
            splitButtonClicked = false;
            armyText.UpdateText(splittedArmyPart);
        }

        private void MoveChosen(IntVector2 targetPosition)
        {
            //TODO: maybe we should disable the whole menu here
            boardStorage.DisableBoardButtons();
            
            var targetObject = boardStorage.GetBoardButton(targetPosition).gameObject;
            boardStorage.SetItem(chosenArmyPosition, null);
            currentTargetPosition = targetPosition;
            var armyObject = chosenArmyItem.StoredObject;
            currentMover = armyObject.GetComponent<ObjectMover>();
            
            currentMover.PrepareMovement(armyObject);
            currentMover.ReachedTarget += FinishMovement;
            currentMover.MoveTo(targetObject);
        }

        private void FinishMovement()
        {
            currentMover.ReachedTarget -= FinishMovement;   

            boardStorage.EnableBoardButtons();

            var resultItem = GetResultItem(boardStorage.GetCurrentBlock(), currentTargetPosition);
            boardStorage.SetItem(currentTargetPosition, resultItem);
            
            //TODO: make castle, pass etc. implement interface IBonusItem so to process them in a common way.
            if (boardStorage.GetBonusItem(currentTargetPosition) is Pass)
            {
                (boardStorage.GetBonusItem(currentTargetPosition) as Pass).PassArmy(resultItem);
            }
            
            if (ProcessAliveArmies())
            {
                ClearMoveState();
                return;
            }
            
            if (ProcessCastle(resultItem.Army))
            {
                ClearMoveState(); 
                return;
            }

            ClearMoveState();        
            FinishedMove?.Invoke();
        }

        private bool ProcessAliveArmies()
        {
            if (!boardStorage.ContainsPlayerArmies(PlayerType.FIRST))
            {
                playGameState.OnFinishGame(ResultType.SECOND_WIN);
                return true;
            }
   
            if (!boardStorage.ContainsPlayerArmies(PlayerType.SECOND))
            {
                playGameState.OnFinishGame(ResultType.FIRST_WIN);
                return true;
            }

            return false;
        }

        private void ClearMoveState()
        {
            SetActiveFrame(null);
            armyText.Clear();
            chosenArmyItem = null;
            currentTargetPosition = null;
            currentMover = null;
        }
        
        private bool ProcessCastle(Army enteredArmy)
        {
            if (boardStorage.IsCastle(currentTargetPosition))
            {
                var castle = boardStorage.GetCastle(currentTargetPosition);
                return castle.PerformAction(enteredArmy);
            }
            return false;
        }

        private ArmyStorageItem GetResultItem(SingleBoardStorage targetBlock, IntVector2 targetPosition)
        {
            while (true)
            {
                if (targetBlock.GetItem(targetPosition) is ArmyStorageItem)
                {
                    var clickedArmyItem = targetBlock.GetItem(targetPosition) as ArmyStorageItem;
                    var resultArmy = clickedArmyItem.Army.PerformAction(chosenArmyItem.Army);
                    if (resultArmy.PlayerType == playerType)
                    {
                        clickedArmyItem.StoredObject.SetActive(false);
                        targetBlock.SetItem(targetPosition, null);
                        chosenArmyItem.Army = resultArmy;
                        return chosenArmyItem;
                    }

                    chosenArmyItem.StoredObject.SetActive(false);
                    targetBlock.SetItem(targetPosition, null);
                    clickedArmyItem.Army = resultArmy;
                    return clickedArmyItem;
                }

                if (!(targetBlock.GetBonusItem(targetPosition) is Pass))
                {
                    return chosenArmyItem;
                }
                var pass = targetBlock.GetBonusItem(targetPosition) as Pass;
                targetBlock = boardStorage.GetBlock(pass.ToBlock);
                targetPosition = pass.ToPosition;
            }
        }

        private bool ReachableFromChosen(IntVector2 position)
        {
            if (chosenArmyItem == null)
            {
                return true;
            }

            return Math.Abs(chosenArmyPosition.x - position.x) + Math.Abs(chosenArmyPosition.y - position.y) == 1;
        }

        /// <summary>
        /// Disables user controller
        /// </summary>
        public void Disable()
        {
            ClearMoveState();
        }

        /// <summary>
        /// Unables user controller
        /// </summary>
        public void Enable()
        {
            boardStorage.EnableArmies(playerType);
        }

        /// <summary>
        /// Finishes user turn
        /// </summary>
        public void FinishTurn()
        {
            ClearMoveState();
            playGameState.OnFinishTurn();
        }

        /// <summary>
        /// Processes action after click on split button
        /// </summary>
        public void OnSplitButtonClick()
        {
            if (chosenArmyItem != null)
            {
                splitButtonClicked = !splitButtonClicked;
            }
        }
    }
}