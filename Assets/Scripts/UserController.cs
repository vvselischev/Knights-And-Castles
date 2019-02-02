using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO: null -> empty object

    //Performs game logic after button click
    public class UserController
    {
        public event VoidHandler FinishedMove;
        private PlayGameState playGameState;
        private BoardStorage boardStorage;
        private PlayerType playerType;
        private ArmyStorageItem chosenArmyItem;
        private Vector2 chosenArmyPosition;
        private bool movementInProgress;
        private bool splitButtonClicked;

        public UserController(PlayerType playerType, Army startArmy,
            BoardStorage storage, PlayGameState playGameState)
        {
            this.playerType = playerType;
            boardStorage = storage;
            this.playGameState = playGameState;
        }

        public void OnButtonClick(int x, int y)
        {
            Debug.Log($"Clicked: ({x}, {y})");
            if (!movementInProgress)
            { 
                ProcessAction(new Vector2(x, y));
            }
        }

        private void ProcessAction(Vector2 position)
        {
            GameObject buttonGO = boardStorage.board.BoardButtons[(int)position.x, (int)position.y].gameObject;
            //Turns off chosenArmyItem.Army activity if it was strange click.
            bool chooseOrMoveClick = false; 
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                ArmyStorageItem clickedArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
                playGameState.armyTextManager.ChangeText(clickedArmyItem.Army.armyComposition.ToString());
                if ((chosenArmyItem == null) && (clickedArmyItem.Army.playerType == playerType) && 
                    (clickedArmyItem.Army as UserArmy).IsActive())
                {
                    chosenArmyItem = clickedArmyItem;
                    chosenArmyPosition = position;
                    chooseOrMoveClick = true;
                    //TODO: light on army icon
                }
            }
            else
            {
                playGameState.armyTextManager.ChangeText("");
                if (chosenArmyItem != null)
                {
                    if (ReachableFromChosen(position) && splitButtonClicked)
                    {
                        Army splittedArmyPart = chosenArmyItem.Army.SplitIntoEqualParts();
                        ArmyStorageItem splittedArmyStorageItem = 
                            new ArmyStorageItem(splittedArmyPart, 
                                InstantiateIcon(chosenArmyItem.StoredObject, position, chosenArmyPosition));
                        MoveChosen(position, buttonGO);
                        boardStorage.boardTable[(int)chosenArmyPosition.x, (int)chosenArmyPosition.y] = splittedArmyStorageItem;
                    }
                }
                else
                {
                    //TODO: turn off icon light
                }
            }

            if ((chosenArmyItem != null) && ReachableFromChosen(position) && (chosenArmyItem.Army as UserArmy).IsActive())
            {
                (chosenArmyItem.Army as UserArmy).setInactive();
                MoveChosen(position, buttonGO);
            }
            else if (!chooseOrMoveClick)
            {
                chosenArmyItem = null;
            }
        }

        private void MoveChosen(Vector2 targetPosition, GameObject targetObject)
        {
            //TODO: Disable UI normally (storage.board.Disable())
            movementInProgress = true;
            boardStorage.SetItem(chosenArmyPosition, null);
            currentTargetPosition = targetPosition;
            GameObject armyObject = chosenArmyItem.StoredObject;
            ObjectMover mover = armyObject.GetComponent<ObjectMover>();
            mover.PrepareMovement(armyObject);
            mover.ReachedTarget += FinishMovement;
            mover.MoveTo(targetObject);
        }

        private Vector2 currentTargetPosition;

        private void FinishMovement()
        {
            if (!movementInProgress)
            {
                return;
            }
            //TODO: Enable UI

            ArmyStorageItem resultItem = GetResultItem();
            boardStorage.SetItem(currentTargetPosition, resultItem);
            chosenArmyItem = null;
            movementInProgress = false;
            FinishedMove?.Invoke();
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

        private bool ReachableFromChosen(Vector2 position)
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
        
        }

        // Set all user armies active
        public void Enable()
        {
            for (int i = 1; i <= boardStorage.board.height; i++)
            {
                for (int j = 1; j <= boardStorage.board.width; j++)
                {
                    if (boardStorage.GetItem(j, i) is ArmyStorageItem)
                    {
                        Army army = (boardStorage.GetItem(j, i) as ArmyStorageItem).Army;
                        if (army.playerType == playerType)
                        {
                            (army as UserArmy).setActive();
                        }
                    }
                }
            }
        }

        public void FinishTurn()
        {
            playGameState.OnFinishTurn(playerType);
        }

        public void OnSplitButtonClick()
        {
            if (chosenArmyItem != null)
            {
                splitButtonClicked = true;
            }
        }

        private GameObject InstantiateIcon(GameObject gameObject, Vector2 to, Vector2 from)
        {
            GameObject newObject = UnityEngine.GameObject.Instantiate(gameObject);
          //  RectTransform rectTransform = newObject.GetComponent<RectTransform>();
         //   rectTransform.position += boardStorage.board.GetOffsetFromPattern((int)to.x, (int)to.y) - 
           //                           boardStorage.board.GetOffsetFromPattern((int)from.x, (int)from.y);
            return newObject;
        }
    }
}