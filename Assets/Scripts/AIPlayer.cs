using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        private class MoveInformation
        {
            public Vector2 targetPosition;
            public Vector2 sourcePosition;
            public int benefit;

            public MoveInformation(Vector2 targetPosition, Vector2 sourcePosition, int benefit)
            {
                this.targetPosition = targetPosition;
                this.sourcePosition = sourcePosition;
                this.benefit = benefit;
            }
        }

        public BoardStorage boardStorage;
        public InputListener inputListener;
        public PlayerType playerType;

        //Move -- move of one of your armies.
        private static int moveNumber;   
        private static int turnNumber;
        private int armyQuantity;

        public void Initialize(UserController controller, PlayerType playerType)
        {
            turnNumber = 0;
            controller.FinishedMove += OnFinishMove;
            this.playerType = playerType;
        }

        public void Activate()
        {
            moveNumber = 0;
            turnNumber++;
            armyQuantity = FindActiveArmies().Count;
            MakeMove();
        }

        private void MakeMove()
        {
            //Analyze board storage...
            //Remember that the board is not inverted, so you start from (6, 6).

            moveNumber++;
            /*            if ((turnNumber == 1) && (moveNumber == 1))
                        {
                            MakeMove(new Vector2(6, 6), new Vector2(6, 5));
                            return;
                            //MakeMove will be called again automatically when icon reaches the target.     
                            //But write return after every call of MakeMove(int, int, int, int) to ensure that it is called only once!
                        } 
                        else if ((turnNumber == 2) && (moveNumber == 1))
                        {
                            MakeMove(new Vector2(6, 5), new Vector2(5, 5));
                            return;
                        }
            */
            if (moveNumber <= armyQuantity)
            {
                MoveInformation moveInformation = AnalyzeBoard();
                MakeMove(moveInformation.sourcePosition, moveInformation.targetPosition);
                return;
            }

            //Don't forget to finish turn! 
            //Just leave it here and if there is a return statement after each MakeMove, you don't need to care.
            FinishTurn();
        }

        private void MakeMove(Vector2 from, Vector2 to)
        {
            //Don't forget, that you must control the correctness of moves!
            //TODO: check the correctness.
            inputListener.ProcessBoardClick(boardStorage.GetBoardButton((int)from.x, (int)from.y));
            inputListener.ProcessBoardClick(boardStorage.GetBoardButton((int)to.x, (int)to.y));
        }

        private MoveInformation AnalyzeBoard() {
            int maxBenefit = -1; // If it is allowed to skip turn, however to stay is worse than to move
            List<Vector2> aiArmies = FindActiveArmies();
            Vector2 targetPosition = aiArmies[0]; // Exception, when AI armies are killed
            Vector2 movingArmyPosition = aiArmies[0]; // Exception, when AI armies are killed

            foreach (Vector2 position in aiArmies)
            {
                MoveInformation moveInformation = AnalyzeBoard(position);

                if (moveInformation.benefit > maxBenefit)
                {
                    maxBenefit = moveInformation.benefit;
                    targetPosition = moveInformation.targetPosition;
                    movingArmyPosition = position;
                }
            }

            return new MoveInformation(targetPosition, movingArmyPosition, maxBenefit);
        }

        private MoveInformation AnalyzeBoard(Vector2 armyPosition)
        {
            // primitive variant of friendly AI (Does not want to fight)
            List<Vector2> possibleTargetPositions = PossibleTargetPositions(armyPosition);
            List<MoveInformation> possibleResults = new List<MoveInformation>();

            foreach (Vector2 position in possibleTargetPositions)
            {
                double armyPower = (boardStorage.GetItem(armyPosition) as ArmyStorageItem).Army.armyComposition.ArmyPower();
                possibleResults.Add(new MoveInformation(position, armyPosition, CalcBenefit(position, armyPower)));
            }

            return ChooseBestResult(possibleResults);
        }

        private int CalcBenefit(Vector2 position, double armyPower)
        {
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                Army army = (boardStorage.GetItem(position) as ArmyStorageItem).Army;
                if (army.armyType == ArmyType.NEUTRAL_FRIENDLY)
                {
                    return army.armyComposition.TotalUnitQuantity();
                }
                else if (army.playerType == PlayerType.FIRST &&
                    army.armyComposition.ArmyPower() < armyPower) // works only for AI as a second player
                {
                    return int.MaxValue;
                }
                else
                {
                    return -army.armyComposition.TotalUnitQuantity();
                }
            }
            else
            {
                return 0;
            }
        }

        private MoveInformation ChooseBestResult(List<MoveInformation> possibleResults)
        {
            MoveInformation bestResult = possibleResults[0]; // List is not empty
            foreach (MoveInformation result in possibleResults)
            {
                if (result.benefit > bestResult.benefit)
                {
                    bestResult = result;
                }
            }

            return bestResult;
        }

        private List<Vector2> PossibleTargetPositions(Vector2 position)
        {
            List<Vector2> possibleTargetPositions = new List<Vector2>();
            int boardHeight = boardStorage.board.height + 1;
            int boardWidth = boardStorage.board.width + 1;

            if (position.y > 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x, position.y - 1));
            }

            if (position.x > 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x - 1, position.y));
            }

            if (position.y < boardWidth - 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x, position.y + 1));
            }

            if (position.x < boardHeight - 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x + 1, position.y));
            }

            return possibleTargetPositions;
        }

        private List<Vector2> FindActiveArmies() {
            List<Vector2> aiArmyPositions = new List<Vector2>();

            for (int i = 1; i <= boardStorage.board.height; i++)
            {
                for (int j = 1; j <= boardStorage.board.width; j++)
                {
                    Vector2 position = new Vector2(i, j);
                    if (boardStorage.GetItem(position) is ArmyStorageItem)
                    {
                        Army army = (boardStorage.GetItem(position) as ArmyStorageItem).Army;
                        if (army.playerType == playerType)
                        {
                            if (army is UserArmy && (army as UserArmy).IsActive())
                            {
                                aiArmyPositions.Add(position);
                            }
                        }
                    }
                }
            }

            return aiArmyPositions;
        }

        private void OnFinishMove()
        {
            MakeMove();
        }

        private void FinishTurn()
        {
            inputListener.ProcessFinishTurnClick();
        }
    }
}