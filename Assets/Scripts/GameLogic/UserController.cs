﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    //TODO: null -> empty object as an indicator of an empty cell or a defeated army
    
    /// <summary>
    /// Performs game logic after button click.
    /// </summary>
    public class UserController
    {
        /// <summary>
        /// Rises this event on finish move of one army.
        /// </summary>
        public event VoidHandler ArmyFinishedMove;
        /// <summary>
        /// Factory to create new icons on split.
        /// </summary>
        private BoardFactory boardFactory;
        /// <summary>
        /// Play game state to notify when the turn is over.
        /// </summary>
        private PlayGameState playGameState;
        /// <summary>
        /// Type of the player that owns this controller.
        /// </summary>
        private PlayerType playerType;
        /// <summary>
        /// Text of the clicked army to update.
        /// </summary>
        private ArmyText armyText;
        /// <summary>
        /// To use effects during the game.
        /// </summary>
        private RoundEffects roundEffects;
        
        private BlockBoardStorage boardStorage;
        
        /// <summary>
        /// State of move to be saved between clicks or during the army movement.
        /// </summary>
        private ArmyStorageItem chosenArmyItem;
        private IntVector2 chosenArmyPosition;
        private ObjectMover currentMover;
        private IntVector2 currentTargetPosition;
        private bool splitButtonClicked;

        /// <summary>
        /// Max number of player armies on board.
        /// </summary>
        private const int MAX_ARMIES = 5;

        public UserController(PlayerType playerType, BlockBoardStorage storage, BoardFactory boardFactory, 
            PlayGameState playGameState, ArmyText armyText, RoundEffects roundEffects)
        {
            this.playerType = playerType;
            boardStorage = storage;
            this.boardFactory = boardFactory;
            this.playGameState = playGameState;
            this.armyText = armyText;
            this.roundEffects = roundEffects;
            roundEffects.Initialize(storage);
        }

        /// <summary>
        /// Processes the action after click on board cell with given coordinates  
        /// </summary>
        public void OnButtonClick(int x, int y)
        {
            ProcessAction(new IntVector2(x, y));
        }

        /// <summary>
        /// Processes the single click.
        /// Updates move state if necessary. 
        /// </summary>
        private void ProcessAction(IntVector2 position)
        {
            var chooseOrMoveClick = false;
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                //Click on a cell with an army.
                if (!ProcessClickOnArmy(position, ref chooseOrMoveClick))
                {
                    //Nothing to do, repeated click on the same army.
                    return;
                }
            }
            else
            {
                //Click on an empty cell.
                armyText.Clear();
                SetActiveFrame(null);
                if (ProcessSplitClickOnAdjacent(position))
                {
                    //It was a click on an empty cell in a split mode, already processed.
                    return;
                }
            }

            if (ProcessMoveClickOnAdjacent(position))
            {
                //It was a click on adjacent with the chosen cell, so army move was processed.
                //Note that adjacent cell can be both empty or with an army.
                //That is why we have this condition separate with the previous one.
                return;
            }

            if (!chooseOrMoveClick)
            {
                //It was not click to choose or move an army.
                //So drop the current state.
                chosenArmyItem = null;
                chosenArmyPosition = null;
                
                 //We process the pass only if it was not move click on it.
                if (boardStorage.GetBonusItem(position) is Pass)
                {
                    //It was a simple click on a pass since we have already processed any army move.
                    //So just change the current block.
                    var pass = boardStorage.GetBonusItem(position) as Pass;
                    pass.ChangeBlock();
                }
            }

            //Split mode is active only during the next click.
            SetSplitModeActive(false);
        }

        /// <summary>
        /// Processes the click on a cell with the army.
        /// If it was a repeated click, clears the move state and returns false.
        /// Otherwise activates the frame across the chosen army icon, updates the army description text
        /// and updates the move state appropriately (see ProcessChooseClick), and returns true.
        /// Determines, whether it was choose or move click;
        /// </summary>
        private bool ProcessClickOnArmy(IntVector2 position, ref bool chooseOrMoveClick)
        {
            if (ProcessClickOnSameCell(position))
            {
                return false;
            }

            SetActiveFrame(position);
            var clickedArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
            armyText.UpdateText(clickedArmyItem.Army);

            chooseOrMoveClick = ProcessChooseClick(position, clickedArmyItem);
            return true;
        }

        /// <summary>
        /// Determines whether the click was a chose click and remembers the clicked army, if so
        /// and if this army belongs to the player and is active.
        /// If it was an enemy army or an inactive army, drops the possibly previously chosen army.
        /// If it was not a choose click, nothing changes.
        /// Returns true if it was choose or move click.
        /// </summary>
        private bool ProcessChooseClick(IntVector2 position, ArmyStorageItem clickedArmyItem)
        {
            if (clickedArmyItem.Army.PlayerType != playerType || !((UserArmy) clickedArmyItem.Army).IsActive())
            {
                //A clicked army does not belong to the player or is inactive. 
                return false;
            }

            if (chosenArmyItem != null && ReachableFromChosen(position))
            {
                //It was a move click. Will be processed further.
                return true;
            }
            
            //Remember the clicked army, it was a choose click.
            chosenArmyItem = clickedArmyItem;
            chosenArmyPosition = position;
            return true;
        }

        /// <summary>
        /// Checks that it was the click to move a previously chosen army.
        /// If so, returns true and performs a move.
        /// Otherwise returns false.
        /// While performing a move, moving army becomes inactive.
        /// </summary>
        private bool ProcessMoveClickOnAdjacent(IntVector2 position)
        {
            if (chosenArmyItem == null || !ReachableFromChosen(position) ||
                !(chosenArmyItem.Army as UserArmy).IsActive() || splitButtonClicked)
            {
                return false;
            }
            
            //An army was chosen, position of the current click is reachable from the chosen one,
            //chosen army is active and we are not in a split mode.
            (chosenArmyItem.Army as UserArmy).SetInactive();
            MoveChosen(position);
            return true;
        }

        /// <summary>
        /// Checks the necessary conditions to perform split.
        /// If satisfied, returns true and performs split.
        /// Otherwise, returns false.
        /// If split is performed, the frame moves to the newly created army.
        /// </summary>
        private bool ProcessSplitClickOnAdjacent(IntVector2 position)
        {
            //We must have a chosen army, active split mode,
            //target position should be reachable from the chosen one and should not be a pass,
            //we must not overcome the limit of our armies on the board.
            if (chosenArmyItem == null || 
                !splitButtonClicked ||
                boardStorage.GetBonusItem(position) is Pass ||
                !ReachableFromChosen(position))
            {
                
                return false;
            }

            if (boardStorage.FindPlayerArmies(playerType).Count >= MAX_ARMIES)
            {
                //An attempt to create too many armies.
                armyText.DisplayMaximumArmiesOnBoard();
                return false;
            }

            //All conditions satisfied, perform split.
            ProcessSplit(chosenArmyPosition, position);
            SetActiveFrame(position);
            SetSplitModeActive(false);
            chosenArmyItem = null;
            chosenArmyPosition = null;
            return true;
        }

        /// <summary>
        /// Checks that it was a click on the same cell as the previous one.
        /// If so, drops the whole move state and returns true.
        /// Otherwise, returns false.
        /// </summary>
        private bool ProcessClickOnSameCell(IntVector2 position)
        {
            if (chosenArmyPosition == null || !chosenArmyPosition.Equals(position))
            {
                return false;
            }
            
            ClearMoveState();
            return true;
        }

        /// <summary>
        /// Transfers the current chosen position to the effects service.
        /// </summary>
        private void SetActiveFrame(IntVector2 position)
        {
            roundEffects.SetChosenArmyFrame(position);
        }
 
        /// <summary>
        /// Performs a split of the army on the given chosen position to the given target position.
        /// Split is performed into equal parts.
        /// Assuming that all checks were performed previously.
        /// Updates the army description text to the newly created army.
        /// </summary>
        private void ProcessSplit(IntVector2 chosenPosition, IntVector2 targetPosition)
        {
            //Get new army's composition.
            var splittedArmyPart = chosenArmyItem.Army.SplitIntoEqualParts();
            //Clone old army's icon.
            var clonedIcon = boardFactory.CloneBoardIcon(boardStorage, chosenPosition.x, chosenPosition.y);
            //Place it to the target position in the storage. The position on the screen will be updated 
            //automatically by the storage.
            boardStorage.SetItem(targetPosition.x, targetPosition.y, new ArmyStorageItem(splittedArmyPart, clonedIcon));
            
            //Turn off the split mode and update the text.
            SetSplitModeActive(false);
            armyText.UpdateText(splittedArmyPart);
        }

        /// <summary>
        /// Starts a movement of the chosen army to the given position.
        /// Board buttons are disabled.
        /// </summary>
        private void MoveChosen(IntVector2 targetPosition)
        {
            //TODO: maybe we should disable the whole menu here? It will remove the extra dependency in BoardStorage.
            boardStorage.DisableBoardButtons();
            
            //Removes an item from the old position.
            boardStorage.SetItem(chosenArmyPosition, null);
            //Save target position for further processing after icon will reach the target.
            currentTargetPosition = targetPosition;
            //Get mover component of the chosen army.
            var armyObject = chosenArmyItem.StoredObject;
            currentMover = armyObject.GetComponent<ObjectMover>();
            
            //Initialize the mover, subscribe on its finish event.
            currentMover.PrepareMovement(armyObject);
            currentMover.ReachedTarget += OnFinishMovement;
            //Get the target button object and start movement.
            var targetObject = boardStorage.GetBoardButton(targetPosition).gameObject;
            currentMover.MoveTo(targetObject);
        }

        /// <summary>
        /// Processes an appropriate action after an army reaches the target cell.
        /// </summary>
        private void OnFinishMovement()
        {
            //Unsubscribe (otherwise, this method may be called several times the next time).
            currentMover.ReachedTarget -= OnFinishMovement;

            //Get the result item on the target cell and set it to the board storage.
            //Here the possible fight is performed, even if is a move through the pass
            //and an enemy army blocked it.
            var resultItem = GetResultItem(boardStorage.GetCurrentBlock(), currentTargetPosition);
            boardStorage.SetItem(currentTargetPosition, resultItem);
            
            //TODO: make castle, pass etc. implement interface IBonusItem so to process them in a common way.
            if (boardStorage.GetBonusItem(currentTargetPosition) is Pass)
            {
                //We moved to the pass, use it to transfer our army.
                (boardStorage.GetBonusItem(currentTargetPosition) as Pass).PassArmy(resultItem);
            }
            
            //Check if some player lost all its armies.
            //If so, return, since the game is over.
            if (CheckAliveArmies())
            {
                ClearMoveState();
                return;
            }
            
            //Check if the army reaches the enemy castle.
            //If so, return, since the game is over.
            if (ProcessCastle(resultItem.Army))
            {
                ClearMoveState(); 
                return;
            }
            //At first we check alive armies, since the last player's army may be defeated
            //at the cell with an enemy castle and should not win.
            
            //We finished the army move, clear the move state.
            ClearMoveState(); 
            
            //EnablePlayerEffects buttons.
            boardStorage.EnableBoardButtons();
            
            //Signalize that we finish the move of one army.
            ArmyFinishedMove?.Invoke();
        }

        /// <summary>
        /// Checks that no armies of one of the players exist on the board.
        /// If so, notifies play game state with the corresponding result type
        /// and returns true.
        /// Otherwise, returns false.
        /// </summary>
        private bool CheckAliveArmies()
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

        /// <summary>
        /// Clears all move state.
        /// Clears the army text and disables the current frame (if exists).
        /// </summary>
        private void ClearMoveState()
        {
            SetActiveFrame(null);
            armyText.Clear();
            chosenArmyItem = null;
            currentTargetPosition = null;
            currentMover = null;
            chosenArmyPosition = null;
            SetSplitModeActive(false);
        }
        
        /// <summary>
        /// Checks that the target position of an army move was castle.
        /// If so, performs an action with this castle and returns the result of this action.
        /// Otherwise, returns false.
        /// </summary>
        private bool ProcessCastle(Army enteredArmy)
        {
            if (!(boardStorage.GetBonusItem(currentTargetPosition) is Castle))
            {
                return false;
            }

            var castle = boardStorage.GetBonusItem(currentTargetPosition) as Castle;
            return castle.PerformAction(enteredArmy);
        }

        /// <summary>
        /// Returns the army that is the result of the action after the chosen army reaches the target cell.
        /// Note, that the target position may be a pass, so this method recursively looks at the cell
        /// outside this pass.
        /// Moreover, the outside cell may also contain a pass, so the searching is repeated.
        /// </summary>
        private ArmyStorageItem GetResultItem(SingleBoardStorage targetBlock, IntVector2 targetPosition)
        {
            //Searching for the first cell on the possible sequence of passes starting at the target position,
            //that is not a pass. 
            while (true)
            {
                if (targetBlock.GetItem(targetPosition) is ArmyStorageItem)
                {
                    //The target position contains an army.
                    //Get that army item.
                    var clickedArmyItem = targetBlock.GetItem(targetPosition) as ArmyStorageItem;
                    //Perform an action between our army and an army on the target cell.
                    var resultArmy = clickedArmyItem.Army.PerformAction(chosenArmyItem.Army);
                    
                    if (resultArmy.PlayerType == playerType)
                    {
                        //The result of the action is our army. Delete another army and return ours.
                        clickedArmyItem.StoredObject.SetActive(false);
                        targetBlock.SetItem(targetPosition, null);
                        chosenArmyItem.Army = resultArmy;
                        return chosenArmyItem;
                    }
                    //The result is not our army. Delete our army and return another.
                    chosenArmyItem.StoredObject.SetActive(false);
                    targetBlock.SetItem(targetPosition, null);
                    clickedArmyItem.Army = resultArmy;
                    return clickedArmyItem;
                }

                if (!(targetBlock.GetBonusItem(targetPosition) is Pass))
                {
                    //The target position is not a pass, so our army simply transfers, and the result is our army.
                    return chosenArmyItem;
                }
                
                //The target position contains a pass. Update the target block and position and continue.
                var pass = targetBlock.GetBonusItem(targetPosition) as Pass;
                targetBlock = boardStorage.GetBlock(pass.ToBlock);
                targetPosition = pass.ToPosition;
            }
        }

        /// <summary>
        /// Returns true if an army was chosen and the given position is adjacent to the chosen one.
        /// </summary>
        private bool ReachableFromChosen(IntVector2 position)
        {
            if (chosenArmyItem == null)
            {
                return true;
            }
            return Math.Abs(chosenArmyPosition.x - position.x) + Math.Abs(chosenArmyPosition.y - position.y) == 1;
        }

        /// <summary>
        /// Method is called when turning off this controller.
        /// Clears move state.
        /// </summary>
        public void Disable()
        {
            ClearMoveState();
        }

        /// <summary>
        /// Enables user controller. Sets all player armies active.
        /// </summary>
        public void Enable()
        {
            boardStorage.EnableArmies(playerType);
        }

        /// <summary>
        /// Processes the click on finish turn.
        /// Clears move state and notifies the play game state.
        /// </summary>
        public void OnFinishTurnClick()
        {
            ClearMoveState();
            playGameState.OnFinishTurn();
        }

        /// <summary>
        /// Processes action after click on split button.
        /// Simply remember that it was clicked if a user army was previously chosen.
        /// </summary>
        public void OnSplitButtonClick()
        {
            if (chosenArmyItem != null)
            {
                SetSplitModeActive(true);
            }
            else
            {
                armyText.SelectArmyRequest();
            }
        }

        private void SetSplitModeActive(bool splitModeActive)
        {
            splitButtonClicked = splitModeActive;
            if (splitModeActive)
            {
                roundEffects.EnableSplitMode(GetPossibleForSplitPositions());
            }
            else
            {
                roundEffects.DisableSplitMode();
            }
        }
        
        private List<IntVector2> GetPossibleForSplitPositions()
        {
            if (chosenArmyPosition == null)
            {
                return null;
            }
            
            var possiblePositions = new List<IntVector2>();
            
            //Encode pairs of delta coordinates of positions adjacent to the chosen army.
            int[] deltaX = {-1, 0, 1, 0};
            int[] deltaY = {0, 1, 0, -1};
            for (int i = 0; i < deltaX.Length; i++)
            {
                var adjacentPosition = new IntVector2(chosenArmyPosition.x + deltaX[i],
                    chosenArmyPosition.y + deltaY[i]);
                if (!OnBoard(adjacentPosition))
                {
                    //Position is outside the board.
                    continue;
                }

                if (boardStorage.GetItem(adjacentPosition) == null)
                {
                    //Position is empty.
                    possiblePositions.Add(adjacentPosition);
                }
            }
            return possiblePositions;
        }

        private bool OnBoard(IntVector2 position)
        {
            return position.x >= 1 && position.x <= boardStorage.GetCurrentBlock().GetBoardWidth() &&
                   position.y >= 1 && position.y <= boardStorage.GetCurrentBlock().GetBoardHeight();
        }
    }
}