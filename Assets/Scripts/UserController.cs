using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

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
        private IntVector2 chosenArmyPosition;
        private bool movementInProgress;
        private bool splitButtonClicked;
        private int aliveArmies;
        
        public UserController(PlayerType playerType, BoardStorage storage, BoardFactory boardFactory, PlayGameState playGameState)
        {
            this.playerType = playerType;
            boardStorage = storage;
            this.boardFactory = boardFactory;
            this.playGameState = playGameState;
            var armies = boardStorage.FindPlayerArmies();
            if (armies.ContainsKey(playerType))
            {
                aliveArmies = armies[playerType].Count;
            }
            else
            {
                aliveArmies = 0;
            }
            Debug.Log("Alive " + playerType + ": " + aliveArmies);
        }

        public void OnButtonClick(int x, int y)
        {
            Debug.Log($"Clicked: ({x}, {y})");
            if (!movementInProgress)
            { 
                ProcessAction(new IntVector2(x, y));
            }
        }

        private void ProcessAction(IntVector2 position)
        {
            int positionX = position.x;
            int positionY = position.y;
            
            GameObject buttonGO = boardStorage.board.BoardButtons[positionX, positionY].gameObject;
            //Turns off chosenArmyItem.Army activity if it was strange click.
            bool chooseOrMoveClick = false; 
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                ArmyStorageItem clickedArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
                playGameState.armyTextManager.ChangeText(clickedArmyItem.Army.armyComposition.ToString());
                if ((chosenArmyItem == null) && (clickedArmyItem.Army.playerType == playerType) && 
                    ((UserArmy) clickedArmyItem.Army).IsActive())
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
                        ProcessSplit(chosenArmyPosition.x, chosenArmyPosition.y, positionX, positionY);
                    }
                }
                else
                {
                    //TODO: turn off icon light
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

        private void ProcessSplit(int chosenPositionX, int chosenPositionY, int positionX, int positionY)
        {
            Army splittedArmyPart = chosenArmyItem.Army.SplitIntoEqualParts();
            GameObject clonedIcon = boardFactory.CloneBoardIcon(chosenPositionX, chosenPositionY,
                positionX, positionY);
            boardStorage.SetItem(positionX, positionY, new ArmyStorageItem(splittedArmyPart, clonedIcon));
            aliveArmies++;
            splitButtonClicked = false;
        }

        private void MoveChosen(IntVector2 targetPosition, GameObject targetObject)
        {
            boardStorage.board.DisableBoard();
            movementInProgress = true;
            boardStorage.SetItem(chosenArmyPosition, null);
            currentTargetPosition = targetPosition;
            GameObject armyObject = chosenArmyItem.StoredObject;
            ObjectMover mover = armyObject.GetComponent<ObjectMover>();
            
            mover.PrepareMovement(armyObject);
            mover.ReachedTarget += FinishMovement;
            mover.MoveTo(targetObject);
        }

        private IntVector2 currentTargetPosition;

        private void FinishMovement()
        {
            if (!movementInProgress)
            {
                return;
            }
            boardStorage.board.EnableBoard();

            ArmyStorageItem resultItem = GetResultItem();
            boardStorage.SetItem(currentTargetPosition, resultItem);

            if (resultItem.Army.playerType != playerType)
            {
                aliveArmies--;
            }

            Debug.Log("Alive " + playerType + ": " + aliveArmies);
            if (aliveArmies == 0)
            {
                if (playerType == PlayerType.FIRST)
                {
                    playGameState.OnFinishGame(ResultType.SECOND_WIN);
                }
                else if (playerType == PlayerType.SECOND)
                {
                    playGameState.OnFinishGame(ResultType.FIRST_WIN);
                }
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

        private void ClearMoveState()
        {
            chosenArmyItem = null;
            movementInProgress = false;  
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
                if (clickedArmyItem.Army.playerType == chosenArmyItem.Army.playerType)
                {
                    //They merge
                    aliveArmies--;
                }
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