using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AIPlayer : MonoBehaviour
    {
        private class MoveInformation
        {
            public Vector2 TargetPosition { get; }
            public Vector2 SourcePosition { get; }
            public int Benefit { get; }

            public MoveInformation(Vector2 targetPosition, Vector2 sourcePosition, int benefit)
            {
                TargetPosition = targetPosition;
                SourcePosition = sourcePosition;
                Benefit = benefit;
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
            moveNumber++;
            if (moveNumber <= armyQuantity)
            {
                MoveInformation moveInformation = AnalyzeBoard();
                MakeMove(moveInformation.SourcePosition, moveInformation.TargetPosition);
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
            inputListener.ProcessBoardClick((int)from.x, (int)from.y);
            inputListener.ProcessBoardClick((int)to.x, (int)to.y);
        }

        //TODO: rename so to know something about the returning value
        private MoveInformation AnalyzeBoard() {
            int maxBenefit = -1; // If it is allowed to skip turn, however to stay is worse than to move
            List<Vector2> aiArmies = FindActiveArmies();
            //TODO: throw explicit exception
            Vector2 targetPosition = aiArmies[0]; // Exception, when AI armies are killed
            Vector2 movingArmyPosition = aiArmies[0]; // Exception, when AI armies are killed

            foreach (Vector2 position in aiArmies)
            {
                MoveInformation moveInformation = AnalyzeBoard(position);

                if (moveInformation.Benefit > maxBenefit)
                {
                    maxBenefit = moveInformation.Benefit;
                    targetPosition = moveInformation.TargetPosition;
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
                possibleResults.Add(new MoveInformation(position, armyPosition, CalculateBenefit(position, armyPower)));
            }

            return ChooseBestResult(possibleResults);
        }

        private int CalculateBenefit(Vector2 position, double armyPower)
        {
            if (boardStorage.GetItem(position) is ArmyStorageItem)
            {
                Army army = (boardStorage.GetItem(position) as ArmyStorageItem).Army;
                if (army.armyType == ArmyType.NEUTRAL_FRIENDLY)
                {
                    return army.armyComposition.TotalUnitQuantity();
                }
                if (army.playerType == PlayerType.FIRST && army.armyComposition.ArmyPower() < armyPower) // works only for AI as a second player
                {
                    return int.MaxValue;
                }
                return -army.armyComposition.TotalUnitQuantity();
            }
            return 0;
        }

        private MoveInformation ChooseBestResult(List<MoveInformation> possibleResults)
        {
            MoveInformation bestResult = possibleResults[0]; // List is not empty
            foreach (MoveInformation result in possibleResults)
            {
                if (result.Benefit > bestResult.Benefit)
                {
                    bestResult = result;
                }
            }
            return bestResult;
        }

        private List<Vector2> PossibleTargetPositions(Vector2 position)
        {
            List<Vector2> possibleTargetPositions = new List<Vector2>();
            int boardHeight = boardStorage.board.height;
            int boardWidth = boardStorage.board.width;

            if (position.y > 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x, position.y - 1));
            }

            if (position.x > 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x - 1, position.y));
            }

            if (position.y < boardHeight - 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x, position.y + 1));
            }

            if (position.x < boardWidth - 1)
            {
                possibleTargetPositions.Add(new Vector2(position.x + 1, position.y));
            }

            return possibleTargetPositions;
        }

        private List<Vector2> FindActiveArmies() 
        {
            List<Vector2> aiArmyPositions = new List<Vector2>();

            for (int i = 1; i <= boardStorage.board.height; i++)
            {
                for (int j = 1; j <= boardStorage.board.width; j++)
                {
                    Vector2 position = new Vector2(j, i);
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
            //FinishTurn();
           MakeMove();
        }

        private void FinishTurn()
        {
            inputListener.ProcessFinishTurnClick();
        }
    }
}