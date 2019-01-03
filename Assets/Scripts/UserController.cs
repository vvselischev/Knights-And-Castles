using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    //TODO: null -> empty object
    public class UserController
    {
        private PlayGameState playGameState;
        private BoardStorage boardStorage;
        private TurnType turnType;

        private ArmyStorageItem chosenArmyItem;
        private Vector2 chosenArmyPosition;
        private bool movement;

        private Dictionary<Vector2, Army> userArmies = new Dictionary<Vector2, Army>();

        public UserController(TurnType turnType, Vector2 startPosition, Army startArmy,
            BoardStorage storage, PlayGameState playGameState)
        {
            this.turnType = turnType;
            boardStorage = storage;
            userArmies.Add(startPosition, startArmy);
            this.playGameState = playGameState;
        }

        public void OnButtonClick(BoardButton boardButton)
        {
            /*if (boardButton.boardX == 1)
            {
                FinishTurn();
            }*/
            if (!movement)
            { 
                ProcessAction(new Vector2(boardButton.boardX, boardButton.boardY));
            }
        }

        private void ProcessAction(Vector2 position)
        {
            GameObject buttonGO = boardStorage.board.BoardButtons[(int)position.x, (int)position.y].gameObject;
            if (userArmies.ContainsKey(position))
            {
                if (chosenArmyItem == null)
                { 
                    chosenArmyItem = boardStorage.GetItem(position) as ArmyStorageItem;
                    chosenArmyPosition = position;
                    //light on army icon
                } //TODO: show army composition on click
            }
            else if (chosenArmyItem != null)
            { 
                
            }
            else
            {
                //turn off icon light
            }

            if ((chosenArmyItem != null) &&  ReachableFromChosen(position))
            { 
                MoveChosen(position, buttonGO);
            }
        }

        private void MoveChosen(Vector2 targetPosition, GameObject targetObject)
        {
            //TODO: Disable UI
            movement = true;
            userArmies.Remove(chosenArmyPosition);
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
            //TODO: Enable UI
            //Perform action
            if (movement)
            { 
                ArmyStorageItem resultItem = GetResultItem();
                userArmies.Add(currentTargetPosition, resultItem.Army);
                boardStorage.SetItem(currentTargetPosition, resultItem);
                chosenArmyItem = null;
                movement = false;
            }
        }

        private ArmyStorageItem GetResultItem()
        {
            chosenArmyItem.BoardButton = boardStorage.GetBoardButton(currentTargetPosition);
            return chosenArmyItem;
        }

        private bool ReachableFromChosen(Vector2 position)
        {
            if (chosenArmyItem == null)
            {
                return true;
            }
            return (Math.Abs(chosenArmyPosition.x - position.x) + 
                Math.Abs(chosenArmyPosition.y - position.y) == 1);
        }


        public void Disable()
        {
        }

        public void Enable()
        {
        }

        public void FinishTurn()
        {
            playGameState.OnFinishTurn(turnType);
        }
    }
}