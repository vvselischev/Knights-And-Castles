using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameSimulation
    {
        private class ItemAndPosition
        {
            public readonly ArmyStorageItem Item;
            public readonly Cell Cell;
            public ItemAndPosition(ArmyStorageItem item, Cell cell)
            {
                Item = item;
                Cell = cell;
            }
        }
        
        private IBoardStorage boardStorage;

        public GameSimulation(IBoardStorage boardStorage)
        {
            this.boardStorage = boardStorage.CreateSimulationStorage();
        }

        private Dictionary<PlayerType, List<Cell>> FindPlayerArmies()
        {
            var armiesByType = new Dictionary<PlayerType, List<Cell>>
            {
                {PlayerType.FIRST, boardStorage.FindPlayerArmies(PlayerType.FIRST)},
                {PlayerType.SECOND, boardStorage.FindPlayerArmies(PlayerType.SECOND)}
            };


            return armiesByType;
        }

        private double AnalyzePosition(List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells)
        {
            var currentPlayerPositionProfit = CalcPlayerPositionProfit(currentPlayerArmyCells);
            var otherPlayerPositionProfit = CalcPlayerPositionProfit(otherPlayerArmyCells);

            return currentPlayerPositionProfit - otherPlayerPositionProfit;
        }

        private double CalcPlayerPositionProfit(List<Cell> currentPlayerArmyCells)
        {
            double result = 0;

            if (currentPlayerArmyCells.Count == 0)
            {
                return 0;
            }

            var playerType = GetPlayerTypeByCell(currentPlayerArmyCells[0]);

            foreach (var cell in currentPlayerArmyCells)
            {
                var boardItem = boardStorage.GetItem(cell);
                if (boardItem is ArmyStorageItem item)
                {
                    result += CalcArmyPositionProfit(item.Army.ArmyPower(),
                        boardStorage.GetDistanceToEnemyCastle(cell, playerType));
                }
            }

            return result;
        }

        private PlayerType GetPlayerTypeByCell(Cell cell)
        {
            var boardItem = boardStorage.GetItem(cell);
            if (boardItem is ArmyStorageItem item)
            {
                if (item.Army == null)
                {
                    throw new ArgumentException("Army does not exist on this position");
                }

                var army = item.Army;

                if (army is UserArmy userArmy)
                {
                    return userArmy.PlayerType;
                }

                throw new ArgumentException("Not UserArmy on this position");
            }

            throw new ArgumentException("Not ArmyStorageItem");
        }

        private double CalcArmyPositionProfit(double armyPower, int distanceToEnemyCastle)
        {
            if (distanceToEnemyCastle == 0)
            {
                return double.PositiveInfinity; // Player wins
            }

            return armyPower / distanceToEnemyCastle;
        }

        private PlayerType GetOppositePlayerType(PlayerType playerType)
        {
            if (playerType == PlayerType.FIRST)
            {
                return PlayerType.SECOND;
            }

            if (playerType == PlayerType.SECOND)
            {
                return PlayerType.FIRST;
            }
            
            throw new ArgumentException("such playerType is not allowed");
        }

        public MoveInformation FindBestMove(PlayerType playerType)
        {
            //TODO: do it simultaneously.
            var currentPlayerArmyCells = FindPlayerArmies()[playerType];
            var otherPlayerArmyCells = FindPlayerArmies()[GetOppositePlayerType(playerType)];

            int depth;
            if (currentPlayerArmyCells.Count + otherPlayerArmyCells.Count >= 4)
            {
                depth = 3;
            }
            else
            {
                depth = 5;
            }
            
            return AnalyzeStrategy(playerType, true, depth, currentPlayerArmyCells, 
                otherPlayerArmyCells).Item2;
        }

        private Tuple<double, MoveInformation> AnalyzeStrategy(PlayerType playerType, bool isFirstTurn,
            int depth, List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells)
        {
            if (depth == 0)
            {
                return new Tuple<double, MoveInformation>(AnalyzePosition(
                    currentPlayerArmyCells, otherPlayerArmyCells), null);
            }

            var possibleMoves = GetListOfMoves(currentPlayerArmyCells, isFirstTurn);
            var resultBenefit = double.PositiveInfinity;
            MoveInformation bestMoveInformation = null; // not move

            if (possibleMoves == null)
            {
                throw new ArgumentException("no possible moves"); // strange exception
            }

            foreach (var moveInformation in possibleMoves)
            {
                var intermediateResult = MakeAnalyzingMoves(moveInformation, depth, playerType, 
                    currentPlayerArmyCells, otherPlayerArmyCells);

                var distanceEnemyCastleTo = boardStorage.GetDistanceToEnemyCastle(moveInformation.To, playerType);
                var distanceEnemyCastleFrom = boardStorage.GetDistanceToEnemyCastle(moveInformation.From, playerType);
                
                if (distanceEnemyCastleTo == 0) 
                {
                    resultBenefit = playerType == PlayerType.FIRST ? double.PositiveInfinity : double.NegativeInfinity; 

                    bestMoveInformation = moveInformation; 
                    break; 
                }
                
                //Enemy will lose
                if (double.IsNegativeInfinity(intermediateResult.Item1) && distanceEnemyCastleTo < distanceEnemyCastleFrom)
                {
                    resultBenefit = intermediateResult.Item1;
                    bestMoveInformation = moveInformation;
                    break;
                }
                
                if (intermediateResult.Item1 < resultBenefit || 
                    Math.Abs(intermediateResult.Item1 - resultBenefit) < 0.00001 
                    && (bestMoveInformation == null || 
                        boardStorage.GetDistanceToEnemyCastle(bestMoveInformation.To, playerType) > distanceEnemyCastleTo)) 
                { 
                    resultBenefit = intermediateResult.Item1; 
                    bestMoveInformation = moveInformation; 
                }
            }
            
            return new Tuple<double, MoveInformation>(resultBenefit, bestMoveInformation);
        }

        private Tuple<double, MoveInformation> MakeAnalyzingMoves(MoveInformation moveInformation, int depth, 
            PlayerType playerType,  List<Cell> currentPlayerArmyCells, 
            List<Cell> otherPlayerArmyCells)
        {
            var memorizedFrom = new ItemAndPosition(GetItemByPosition(moveInformation.From), moveInformation.From);
            var memorizedTo = new ItemAndPosition(GetItemByPosition(moveInformation.To), moveInformation.To);
           
            MakeMove(moveInformation.From, moveInformation.To, false);
            currentPlayerArmyCells.Remove(moveInformation.From);
            currentPlayerArmyCells.Remove(moveInformation.To);
            otherPlayerArmyCells.Remove(moveInformation.From);
            otherPlayerArmyCells.Remove(moveInformation.To);

            var resultItem = boardStorage.GetItem(moveInformation.To) as ArmyStorageItem;

            if (resultItem.Army != null && resultItem.Army is UserArmy userArmy)
            {
                if (playerType == userArmy.PlayerType)
                {
                    currentPlayerArmyCells.Add(moveInformation.To);
                }
                else
                {
                    otherPlayerArmyCells.Add(moveInformation.To);
                }
            }

            Tuple<double, MoveInformation> result; 
            if (otherPlayerArmyCells.Count == 0) 
            { 
                if (playerType == PlayerType.FIRST) 
                { 
                    result = new Tuple<double, MoveInformation>(double.PositiveInfinity, null); 
                } 
                else 
                { 
                    result = new Tuple<double, MoveInformation>(double.NegativeInfinity, null); 
                } 
            } 
            else if (currentPlayerArmyCells.Count == 0) 
            { 
                if (playerType == PlayerType.FIRST) 
                { 
                    result = new Tuple<double, MoveInformation>(double.NegativeInfinity, null); 
                } 
                else 
                { 
                    result = new Tuple<double, MoveInformation>(double.PositiveInfinity, null); 
                } 
            } 
            else 
            { 
                result = AnalyzeStrategy(GetOppositePlayerType(playerType), false, 
                    depth - 1, otherPlayerArmyCells, 
                    currentPlayerArmyCells); 
            }

            CancelMove(memorizedFrom, memorizedTo);
            currentPlayerArmyCells.Remove(moveInformation.To);
            otherPlayerArmyCells.Remove(moveInformation.To);
            if (memorizedFrom.Item?.Army != null)
            {
                if (memorizedFrom.Item.Army.PlayerType == playerType)
                {
                    currentPlayerArmyCells.Add(moveInformation.From);
                }

                 if (memorizedFrom.Item.Army.PlayerType == GetOppositePlayerType(playerType))
                 {
                    otherPlayerArmyCells.Add(moveInformation.From);
                 }
            }

            if (memorizedTo.Item?.Army != null)
            {
                if (memorizedTo.Item.Army.PlayerType == playerType)
                {
                    currentPlayerArmyCells.Add(moveInformation.To);
                }

                if (memorizedTo.Item.Army.PlayerType == GetOppositePlayerType(playerType))
                {
                    otherPlayerArmyCells.Add(moveInformation.To);
                }
            }

            return result;
        }

        private void CancelMove(ItemAndPosition from, ItemAndPosition to)
        {
            from.Item.Army.SetActive();
            boardStorage.SetItem(from.Cell, from.Item);
            boardStorage.SetItem(to.Cell, to.Item);
        }

        private ArmyStorageItem GetItemByPosition(Cell cell)
        {
            return boardStorage.GetItem(cell) as ArmyStorageItem;
        }

        public void MakeMove(Cell from, Cell to, bool isRealMove) //changes boardStorageEmulation
        {
            var fromItem = boardStorage.GetItem(from) as ArmyStorageItem;
            var toItem = boardStorage.GetItem(to) as ArmyStorageItem;

            
            if (fromItem == null)
            {
                throw new ArgumentException("cannot move nonexistent army");
            }

            boardStorage.SetItem(from, null);
            if (toItem?.Army == null)
            {
                boardStorage.SetItem(to, fromItem);

                if (isRealMove && fromItem.Army is UserArmy userArmy)
                {
                    userArmy.SetInactive();
                }
            }
            else
            {
                var resultArmy = toItem.Army.PerformAction(fromItem.Army);
                
                if (!isRealMove)
                {
                    resultArmy.SetActive();
                }

                boardStorage.SetItem(to, new ArmyStorageItem(resultArmy, null));
            }
        }

        private List<MoveInformation> GetListOfMoves(IEnumerable<Cell> playerArmiesCells, bool isFirstTurn)
        {
            var possibleMoves = new List<MoveInformation>();

            foreach (var cell in playerArmiesCells)
            {
                var boardItem = boardStorage.GetItem(cell);
                if (boardItem is ArmyStorageItem item && item.Army is UserArmy userArmy)
                {
                    if (userArmy.IsActive() || !isFirstTurn)
                    {
                        AddPossibleMoves(possibleMoves, cell);
                    }
                }
            }

            return possibleMoves;
        }
        
        private void AddPossibleMoves(ICollection<MoveInformation> possibleMoves, Cell cell)
        {
            var adjacentCells = boardStorage.GetAdjacent(cell);
            foreach (var adjacentCell in adjacentCells)
            {
                possibleMoves.Add(new MoveInformation(cell, adjacentCell));
            }
        }

        public int GetNumberOfActiveArmies(PlayerType playerType)
        {
            return boardStorage.FindActivePlayerArmies(playerType).Count;
        }
    }
}