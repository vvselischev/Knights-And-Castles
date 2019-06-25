using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Implementation of AI algorithm.
    /// Analyzes recursive tree of moves
    /// </summary>
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

        private static PlayerType aiPlayerType = PlayerType.SECOND;
        private static PlayerType personPlayerType = PlayerType.FIRST;
        
        /// <summary>
        /// Depth of recursion determines the number of moves AI tries to predict.
        /// However, if there are a lot of armies on board, it takes too much time on mobile device to use big depth.
        /// </summary>
        private const int stupidDepth = 3;
        private const int cleverDepth = 5;
        private const int maxArmiesForClever = 3;

        private IBoardStorage boardStorage;
        
        /// <summary>
        /// If positions profit differs less than on epsilon such positions are considered the same
        /// </summary>
        private static double EPSILON = 0.000001;

        public GameSimulation(IBoardStorage boardStorage)
        {
            this.boardStorage = boardStorage.CloneBoardStorage();
        }

        private Dictionary<PlayerType, List<Cell>> FindPlayerArmies()
        {
            var armiesByType = new Dictionary<PlayerType, List<Cell>>
            {
                {personPlayerType, boardStorage.FindPlayerArmies(personPlayerType)},
                {aiPlayerType, boardStorage.FindPlayerArmies(aiPlayerType)}
            };


            return armiesByType;
        }

        /// <summary>
        /// Calculates how current position is profitable for current player
        /// </summary>
        private double AnalyzePosition(List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells)
        {
            var currentPlayerPositionProfit = CalcPlayerPositionProfit(currentPlayerArmyCells);
            var otherPlayerPositionProfit = CalcPlayerPositionProfit(otherPlayerArmyCells);

            return currentPlayerPositionProfit - otherPlayerPositionProfit;
        }

        /// <summary>
        /// Calculates player position profit as summary of all his armies profits.
        /// </summary>
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

        /// <summary>
        /// Calculates position profit for given position
        /// </summary>
        private double CalcArmyPositionProfit(double armyPower, int distanceToEnemyCastle)
        {
            if (distanceToEnemyCastle == 0)
            {
                return double.PositiveInfinity; // Player wins
            }

            return armyPower / distanceToEnemyCastle;
        }

        /// <summary>
        /// FIRST and SECOND player types are supposed to be opposite, if other player type was provided as argument
        /// exception is thrown
        /// </summary>
        private PlayerType GetOppositePlayerType(PlayerType playerType)
        {
            if (playerType == personPlayerType)
            {
                return aiPlayerType;
            }

            if (playerType == aiPlayerType)
            {
                return personPlayerType;
            }
            
            throw new ArgumentException("such playerType is not allowed");
        }

        /// <summary>
        /// Analyzes position and finds move for which guaranteed position profit is max
        /// </summary>
        public MoveInformation FindBestMove(PlayerType playerType)
        {
            //TODO: do it simultaneously.
            var currentPlayerArmyCells = FindPlayerArmies()[playerType];
            var otherPlayerArmyCells = FindPlayerArmies()[GetOppositePlayerType(playerType)];

            int depth;
            if (currentPlayerArmyCells.Count + otherPlayerArmyCells.Count > maxArmiesForClever)
            {
                depth = stupidDepth;
            }
            else
            {
                depth = cleverDepth;
            }
            
            return AnalyzeStrategy(playerType, true, depth, currentPlayerArmyCells, 
                otherPlayerArmyCells).Item2;
        }

        /// <summary>
        /// Analyzes current position and finds move for which guaranteed profit is max
        /// </summary>
        /// <param name="playerType"></param>
        /// <param name="isFirstTurn"> this parameter is important for finding active armies,
        /// if it is false then all armies are supposed to be active</param>
        private Tuple<double, MoveInformation> AnalyzeStrategy(PlayerType playerType, bool isFirstTurn,
            int depth, List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells)
        {
            if (depth == 0)
            {
                return new Tuple<double, MoveInformation>(AnalyzePosition(
                    currentPlayerArmyCells, otherPlayerArmyCells), null);
            }

            var possibleMoves = GetListOfMoves(currentPlayerArmyCells, isFirstTurn);

            if (possibleMoves == null)
            {
                throw new ArgumentException("no possible moves"); // strange exception
            }

            return AnalyzeMoves(playerType, possibleMoves, depth, currentPlayerArmyCells, otherPlayerArmyCells);
        }

        /// <summary>
        /// Among possible moves method chooses the one, which guarantees max position profit
        /// </summary>
        private Tuple<double, MoveInformation> AnalyzeMoves(PlayerType playerType, List<MoveInformation> possibleMoves,
            int depth, List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells)
        {
            var resultBenefit = double.PositiveInfinity; // The best guaranteed profit
            MoveInformation bestMoveInformation = null; // The best move, null means that it is better not to move
            
            foreach (var moveInformation in possibleMoves)
            {
                // Analyzing single move
                var intermediateResult = MakeAnalyzingMoves(moveInformation, depth, playerType, currentPlayerArmyCells, otherPlayerArmyCells);
                var distanceEnemyCastleTo = boardStorage.GetDistanceToEnemyCastle(moveInformation.To, playerType);
                var distanceEnemyCastleFrom = boardStorage.GetDistanceToEnemyCastle(moveInformation.From, playerType);
                
                if (distanceEnemyCastleTo == 0) // If castle was reached
                {
                    resultBenefit = playerType == personPlayerType ? double.PositiveInfinity : double.NegativeInfinity; 
                    bestMoveInformation = moveInformation; 
                    break; 
                }

                // Comparing best and current moves
                if (IsMoveBetter(resultBenefit, intermediateResult.Item1, bestMoveInformation, playerType, distanceEnemyCastleTo))
                {
                    resultBenefit = intermediateResult.Item1;
                    bestMoveInformation = moveInformation;

                    if (IsMovePerfect(resultBenefit, distanceEnemyCastleTo, distanceEnemyCastleFrom))
                    {
                        break;
                    }
                }
            }
            
            return new Tuple<double, MoveInformation>(resultBenefit, bestMoveInformation);
        }

        /// <summary>
        /// Checks that move leads to win in min number of steps
        /// </summary>
        private Boolean IsMovePerfect(double moveBenefit, double distanceEnemyCastleTo, double distanceEnemyCastleFrom)
        {
            return double.IsNegativeInfinity(moveBenefit) && distanceEnemyCastleTo < distanceEnemyCastleFrom;
        }

        /// <summary>
        /// Checks that analyzing move is better than current the best
        /// </summary>
        private Boolean IsMoveBetter(double currentBenefit, double moveBenefit, MoveInformation currentMove, 
            PlayerType playerType, double distanceEnemyCastleTo)
        {
            return moveBenefit < currentBenefit ||
                   Math.Abs(moveBenefit - currentBenefit) < EPSILON
                   && (currentMove == null ||
                        boardStorage.GetDistanceToEnemyCastle(currentMove.To, playerType) > distanceEnemyCastleTo);
        }

        /// <summary>
        /// Make all possible moves and chooses the once which guarantees the max profit 
        /// </summary>
        private Tuple<double, MoveInformation> MakeAnalyzingMoves(MoveInformation moveInformation, int depth, 
            PlayerType playerType,  List<Cell> currentPlayerArmyCells, 
            List<Cell> otherPlayerArmyCells)
        {
            var memorizedFrom = new ItemAndPosition(GetItemByPosition(moveInformation.From), moveInformation.From); // memorizing position
            var memorizedTo = new ItemAndPosition(GetItemByPosition(moveInformation.To), moveInformation.To);
           
            MakeMove(moveInformation.From, moveInformation.To);
            ChangeArmyListAfterMoving(currentPlayerArmyCells, playerType, moveInformation); // changing list of armies after move
            ChangeArmyListAfterMoving(otherPlayerArmyCells, GetOppositePlayerType(playerType), moveInformation);

            Tuple<double, MoveInformation> result; 
            if (PlayerArmiesWereKilled(currentPlayerArmyCells) || PlayerArmiesWereKilled(otherPlayerArmyCells)) // If game ends because of armies of one player were killed
            {
                result = OnOnceArmiesKilled(currentPlayerArmyCells, otherPlayerArmyCells, playerType);
            } 
            else 
            { 
                result = AnalyzeStrategy(GetOppositePlayerType(playerType), false, // Just analyzing the move profit
                    depth - 1, otherPlayerArmyCells, 
                    currentPlayerArmyCells); 
            }

            CancelMove(memorizedFrom, memorizedTo);
            ChangeArmyListAfterCancellingMove(currentPlayerArmyCells, otherPlayerArmyCells, // Changing list of armies after cancelling move
                playerType, moveInformation, memorizedFrom, memorizedTo);

            return result;
        }

        /// <summary>
        /// Checks that no more player armies were left
        /// </summary>
        private Boolean PlayerArmiesWereKilled(List<Cell> armyCells)
        {
            return armyCells.Count == 0;
        }

        /// <summary>
        /// Returns position profit if one of the players loses all armies
        /// </summary>
        private Tuple<double, MoveInformation> OnOnceArmiesKilled(List<Cell> currentPlayerArmyCells, 
            List<Cell> otherPlayerArmyCells, PlayerType playerType)
        {
            if (otherPlayerArmyCells.Count == 0 && playerType == personPlayerType ||
                currentPlayerArmyCells.Count == 0 && playerType == aiPlayerType)
            {
                return new Tuple<double, MoveInformation>(double.PositiveInfinity, null);
            } 
            
            return new Tuple<double, MoveInformation>(double.NegativeInfinity, null);
        }

        /// <summary>
        /// After cancelling move in game simulation all armies should return to their cells and list of
        /// armies should be updated
        /// </summary>
        private void ChangeArmyListAfterCancellingMove(List<Cell> currentPlayerArmyCells, 
            List<Cell> otherPlayerArmyCells, PlayerType playerType, MoveInformation move, 
            ItemAndPosition memorizedFrom, ItemAndPosition memorizedTo)
        {
            currentPlayerArmyCells.Remove(move.To);
            otherPlayerArmyCells.Remove(move.To);
            
            if (memorizedFrom.Item?.Army != null)
            {
                AddCellToArmyCellsList(currentPlayerArmyCells, otherPlayerArmyCells, playerType, 
                    memorizedFrom.Item.Army.PlayerType, move.From);
            }

            if (memorizedTo.Item?.Army != null)
            {
                AddCellToArmyCellsList(currentPlayerArmyCells, otherPlayerArmyCells, playerType, 
                    memorizedTo.Item.Army.PlayerType, move.To);
            }
        }

        /// <summary>
        /// Adds cell to appropriate list of armies due to armies player type on it
        /// </summary>
        private void AddCellToArmyCellsList(List<Cell> currentPlayerArmyCells, List<Cell> otherPlayerArmyCells, 
            PlayerType playerType, PlayerType armyPlayerType, Cell cell)
        {
            if (armyPlayerType == playerType)
            {
                currentPlayerArmyCells.Add(cell);
            }

            if (armyPlayerType == GetOppositePlayerType(playerType))
            {
                otherPlayerArmyCells.Add(cell);
            }
        }

        /// <summary>
        /// After army was moved its position was changed and maybe position of some other armies was changed too
        /// (if it was located on target cell), so list of armies should be updated
        /// </summary>
        private void ChangeArmyListAfterMoving(List<Cell> armyCells, PlayerType playerType, MoveInformation move)
        {
            armyCells.Remove(move.From);
            armyCells.Remove(move.To);

            var resultItem = boardStorage.GetItem(move.To) as ArmyStorageItem;

            if (resultItem?.Army != null && resultItem.Army is UserArmy userArmy)
            {
                if (playerType == userArmy.PlayerType)
                {
                    armyCells.Add(move.To);
                }
            }
        }

        /// <summary>
        /// Cancels move by setting items back
        /// </summary>
        private void CancelMove(ItemAndPosition from, ItemAndPosition to)
        {
            from.Item.Army.SetActive();
            boardStorage.SetItem(from.Cell, from.Item);
            boardStorage.SetItem(to.Cell, to.Item);
        }

        /// <summary>
        /// Returns item from board located on given cell
        /// </summary>
        private ArmyStorageItem GetItemByPosition(Cell cell)
        {
            return boardStorage.GetItem(cell) as ArmyStorageItem;
        }

        /// <summary>
        /// Makes move in simulation from one cell to another
        /// </summary>
        public void MakeMove(Cell from, Cell to)
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
            }
            else
            {
                var resultArmy = toItem.Army.PerformAction(fromItem.Army);
                
                resultArmy.SetActive();
                boardStorage.SetItem(to, new ArmyStorageItem(resultArmy, null));
            }
        }

        /// <summary>
        /// Returns moves to all adjacent cells
        /// </summary>
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
        
        /// <summary>
        /// Adds to collection all moves to adjacent cells
        /// </summary>
        private void AddPossibleMoves(ICollection<MoveInformation> possibleMoves, Cell cell)
        {
            var adjacentCells = boardStorage.GetAdjacent(cell);
            foreach (var adjacentCell in adjacentCells)
            {
                possibleMoves.Add(new MoveInformation(cell, adjacentCell));
            }
        }

        /// <summary>
        /// Returns number of active armies on a board
        /// </summary>
        public int GetNumberOfActiveArmies(PlayerType playerType)
        {
            return boardStorage.FindActivePlayerArmies(playerType).Count;
        }
    }
}