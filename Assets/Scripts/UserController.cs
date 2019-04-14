using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    //TODO: null -> empty object

    //Performs game logic after button click
    public class UserController
    {
        public event VoidHandler FinishedMove;
        private BoardFactory boardFactory;
        private PlayGameState playGameState;
        private IBoardStorage boardStorage;
        private PlayerType playerType;
        private ArmyStorageItem chosenArmyItem;
        private ArmyText armyText;
        
        private IntVector2 chosenArmyPosition;
        private IntVector2 activeFramePosition;
        private ObjectMover currentMover;
        private IntVector2 currentTargetPosition;
        private bool splitButtonClicked;

        public UserController(PlayerType playerType, IBoardStorage storage, BoardFactory boardFactory, PlayGameState playGameState, ArmyText armyText)
        {
            this.playerType = playerType;
            boardStorage = storage;
            this.boardFactory = boardFactory;
            this.playGameState = playGameState;
            this.armyText = armyText;
        }

        public void OnButtonClick(int x, int y)
        {
            Debug.Log($"Clicked: ({x}, {y})");
            ProcessAction(new IntVector2(x, y));
        }

        
        //TODO: refactor and simplify checks
        private void ProcessAction(IntVector2 position)
        {
            int positionX = position.x;
            int positionY = position.y;

            SetActiveFrame(position);
            
            GameObject buttonGO = boardStorage.GetBoardButton(position).gameObject;
            
            //Turns off chosenArmyItem.Army activity if it was strange click.
            bool chooseOrMoveClick = false; 
            
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                ArmyStorageItem clickedArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
                
                armyText.ChangeText(clickedArmyItem.Army.armyComposition.ToString());
                
                if ((chosenArmyItem == null) && (clickedArmyItem.Army.playerType == playerType) && 
                    ((UserArmy) clickedArmyItem.Army).IsActive())
                {
                    chosenArmyItem = clickedArmyItem;
                    chosenArmyPosition = position;
                    chooseOrMoveClick = true;
                }
            }
            else
            {
                playGameState.armyText.ChangeText("");
                if (chosenArmyItem != null && !(boardStorage.GetBonusItem(position) is Pass))
                {
                    if (ReachableFromChosen(position) && splitButtonClicked)
                    {
                        ProcessSplit(chosenArmyPosition.x, chosenArmyPosition.y, positionX, positionY);
                    }
                }
            }

            if ((chosenArmyItem != null) && ReachableFromChosen(position) && (chosenArmyItem.Army as UserArmy).IsActive())
            {
                (chosenArmyItem.Army as UserArmy).SetInactive();
                MoveChosen(position, buttonGO);
            }
            else if (!chooseOrMoveClick)
            {
                chosenArmyItem = null;
                if (boardStorage.GetBonusItem(position) is Pass)
                {
                    var pass = boardStorage.GetBonusItem(position) as Pass;
                    pass.ChangeBlock();
                    SetActiveFrame(null);
                }
            }
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
            Army splittedArmyPart = chosenArmyItem.Army.SplitIntoEqualParts();
            GameObject clonedIcon = boardFactory.CloneBoardIcon(chosenPositionX, chosenPositionY);
            boardStorage.SetItem(positionX, positionY, new ArmyStorageItem(splittedArmyPart, clonedIcon));
            splitButtonClicked = false;
            armyText.ChangeText(splittedArmyPart.armyComposition.ToString());
        }

        private void MoveChosen(IntVector2 targetPosition, GameObject targetObject)
        {
            //TODO: maybe we should disable the whole menu here
            boardStorage.DisableBoardButtons();
            
            boardStorage.SetItem(chosenArmyPosition, null);
            currentTargetPosition = targetPosition;
            GameObject armyObject = chosenArmyItem.StoredObject;
            currentMover = armyObject.GetComponent<ObjectMover>();
            
            currentMover.PrepareMovement(armyObject);
            currentMover.ReachedTarget += FinishMovement;
            currentMover.MoveTo(targetObject);
        }

        private void FinishMovement()
        {
            currentMover.ReachedTarget -= FinishMovement;   

            boardStorage.EnableBoardButtons();

            ArmyStorageItem resultItem = GetResultItem();
            boardStorage.SetItem(currentTargetPosition, resultItem);
            
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
            
            //TODO: make castle, pass etc. implement interface IBonusItem so to process them in a common way
            if (boardStorage.GetBonusItem(currentTargetPosition) is Pass)
            {
                (boardStorage.GetBonusItem(currentTargetPosition) as Pass).PassArmy(chosenArmyItem);
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
            armyText.ChangeText("");
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

        private ArmyStorageItem GetResultItem()
        {
            if (boardStorage.GetItem(currentTargetPosition) is ArmyStorageItem)
            {
                ArmyStorageItem clickedArmyItem = boardStorage.GetItem(currentTargetPosition) as ArmyStorageItem;
                Army resultArmy = clickedArmyItem.Army.PerformAction(chosenArmyItem.Army);
                if (resultArmy.playerType == playerType)
                {
                    clickedArmyItem.StoredObject.SetActive(false);
                    chosenArmyItem.Army = resultArmy;
                }
                else
                {
                    chosenArmyItem.StoredObject.SetActive(false);
                    clickedArmyItem.Army = resultArmy;
                    return clickedArmyItem;
                }
            }
            return chosenArmyItem;
        }

        private bool ReachableFromChosen(IntVector2 position)
        {
            if (chosenArmyItem == null)
            {
                return true;
            }
            return Math.Abs(chosenArmyPosition.x - position.x) + 
                Math.Abs(chosenArmyPosition.y - position.y) == 1;
        }

        public void Disable()
        {
            ClearMoveState();
        }

        public void Enable()
        {
            Debug.Log("enable controller run");
            boardStorage.EnableArmies(playerType);
        }

        public void FinishTurn()
        {
            ClearMoveState();
            playGameState.OnFinishTurn(playerType);
        }

        public void OnSplitButtonClick()
        {
            if (chosenArmyItem != null)
            {
                splitButtonClicked = !splitButtonClicked;
            }
        }
    }
}