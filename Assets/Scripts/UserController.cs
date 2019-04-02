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
        private BoardStorage boardStorage;
        private PlayerType playerType;
        private ArmyStorageItem chosenArmyItem;
        private ArmyText armyText;
        
        private IntVector2 chosenArmyPosition;
        private IntVector2 activeFramePosition;
        private ObjectMover currentMover;
        private IntVector2 currentTargetPosition;
        private bool splitButtonClicked;

        public UserController(PlayerType playerType, BoardStorage storage, BoardFactory boardFactory, PlayGameState playGameState, ArmyText armyText)
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

        private void ProcessAction(IntVector2 position)
        {
            int positionX = position.x;
            int positionY = position.y;

            SetActiveFrame(position);
            
            GameObject buttonGO = boardStorage.board.BoardButtons[positionX, positionY].gameObject;
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
                if (chosenArmyItem != null)
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
            GameObject clonedIcon = boardFactory.CloneBoardIcon(chosenPositionX, chosenPositionY,
                positionX, positionY);
            boardStorage.SetItem(positionX, positionY, new ArmyStorageItem(splittedArmyPart, clonedIcon));
            splitButtonClicked = false;
            armyText.ChangeText(splittedArmyPart.armyComposition.ToString());
        }

        private void MoveChosen(IntVector2 targetPosition, GameObject targetObject)
        {
            //TODO: maybe we should disable the whole menu here
            boardStorage.board.DisableBoard();
            
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

            boardStorage.board.EnableBoard();

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

            ClearMoveState();        
            FinishedMove?.Invoke();
        }

        private bool ProcessAliveArmies()
        {
            var aliveArmies = boardStorage.FindPlayerArmies();
            if (!aliveArmies.ContainsKey(PlayerType.FIRST))
            {
                playGameState.OnFinishGame(ResultType.SECOND_WIN);
                return true;
            }
   
            if (!aliveArmies.ContainsKey(PlayerType.SECOND))
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

        // Set all user armies active
        public void Enable()
        {
            Debug.Log("enable controller run");
            for (int i = 1; i <= boardStorage.board.height; i++)
            {
                for (int j = 1; j <= boardStorage.board.width; j++)
                {
                    if (boardStorage.GetItem(j, i) is ArmyStorageItem)
                    {
                        Army army = ((ArmyStorageItem) boardStorage.GetItem(j, i)).Army;
                        if (army.playerType == playerType)
                        {
                            (army as UserArmy).SetActive();
                        }
                    }
                }
            }
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