using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameSimulation
    {
        private class ItemAndPosition
        {
            public ArmyStorageItemEmulation item;
            public int x;
            public int y;

            public ItemAndPosition(ArmyStorageItemEmulation item, IntVector2 position)
            {
                this.item = item;
                x = position.x;
                y = position.y;
            }
        }
        
        private BoardStorageEmulation boardStorageEmulation;

        public GameSimulation(IBoardStorage boardStorage)
        {
            boardStorageEmulation = new BoardStorageEmulation(boardStorage); 
        }

        private Dictionary<PlayerType, List<IntVector2>> FindPlayerArmies()
        {
            var playerArmyPositions = new Dictionary<PlayerType, List<IntVector2>>();

            for (int i = 1; i <= boardStorageEmulation.Width; i++)
            {
                for (int j = 1; j <= boardStorageEmulation.Height; j++)
                {
                    if (boardStorageEmulation.GetItem(i, j) != null)
                    {
                        var army = boardStorageEmulation.GetItem(i, j).Army;

                        if (army != null)
                        {
                            if (!playerArmyPositions.ContainsKey(army.playerType))
                            {
                                playerArmyPositions.Add(army.playerType, new List<IntVector2>());
                            }
                            playerArmyPositions[army.playerType].Add(new IntVector2(i, j));
                        }
                    }
                }
            }

            return playerArmyPositions;
        }

        private double AnalyzePosition(List<IntVector2> currentPlayerArmyPositions, List<IntVector2> otherPlayerArmyPositions)
        {
            double currentPlayerPositionProfit = CalcPlayerPositionProfit(currentPlayerArmyPositions);
            double otherPlayerPositionProfit = CalcPlayerPositionProfit(otherPlayerArmyPositions);

            return currentPlayerPositionProfit - otherPlayerPositionProfit;
        }

        private double CalcPlayerPositionProfit(List<IntVector2> currentPlayerArmyPositions)
        {
            double result = 0;

            if (currentPlayerArmyPositions.Count == 0)
            {
                return 0;
            }

            var playerType = GetPlayerTypeByPosition(currentPlayerArmyPositions[0]);
            var enemyCastlePosition = FindEnemyCastlePosition(playerType);

            foreach (var position in currentPlayerArmyPositions)
            {
                var army = boardStorageEmulation.GetItem(position.x, position.y).Army;
                result += CalcArmyPositionProfit(army.ArmyPower(), 
                    DistanceBetweenPositions(position, enemyCastlePosition));
            }

            return result;
        }

        private PlayerType GetPlayerTypeByPosition(IntVector2 position)
        {
            var item = boardStorageEmulation.GetItem(position.x, position.y);
            
            if (item?.Army == null)
            {
                throw new ArgumentException("Army does not exist on this position");
            }

            var army = item.Army;

            if (army is UserArmy)
            {
                var userArmy = army as UserArmy;
                return userArmy.playerType;
            }

            throw new ArgumentException("Not UserArmy on this position");
        }

        private static int DistanceBetweenPositions(IntVector2 from, IntVector2 to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }

        private double CalcArmyPositionProfit(double armyPower, int distanceToEnemyCastle)
        {
            if (distanceToEnemyCastle == 0)
            {
                return double.PositiveInfinity; // Player wins
            }

            return armyPower / distanceToEnemyCastle;
        }

        private static PlayerType ChangePlayerType(PlayerType playerType)
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

        //TODO: avoid hardcode constants.
        private IntVector2 FindEnemyCastlePosition(PlayerType playerType)
        {
            if (playerType == PlayerType.FIRST)
            {
                return new IntVector2(boardStorageEmulation.Width, boardStorageEmulation.Height);
            }

            if (playerType == PlayerType.SECOND)
            {
                return new IntVector2(1,1);
            }
            
            throw new ArgumentException("such playerType is not allowed");
        }

        public MoveInformation FindBestMove(PlayerType playerType, int depth)
        {
            //TODO: do it simultaneously.
            var currentPlayerArmyPosition = FindPlayerArmies()[playerType];
            var otherPlayerArmyPosition = FindPlayerArmies()[ChangePlayerType(playerType)];
            
            return AnalyzeStrategy(playerType, true, 
                depth, currentPlayerArmyPosition, otherPlayerArmyPosition).Item2;
        }

        private Tuple<double, MoveInformation> AnalyzeStrategy(PlayerType playerType, bool isFirstTurn,
            int depth, List<IntVector2> currentPlayerArmyPositions, List<IntVector2> otherPlayerArmyPositions)
        {
            if (depth == 0)
            {
                return new Tuple<double, MoveInformation>(AnalyzePosition(
                    currentPlayerArmyPositions, otherPlayerArmyPositions), null);
            }

            var possibleMoves = GetListOfMoves(currentPlayerArmyPositions, isFirstTurn);
            var resultOnWaiting = AnalyzeStrategy(ChangePlayerType(playerType), false,depth - 1, 
                otherPlayerArmyPositions, currentPlayerArmyPositions);
            double resultBenefit = resultOnWaiting.Item1;
            MoveInformation bestMoveInformation = null; // not move

            if (possibleMoves == null)
            {
                throw new ArgumentException("no possible moves"); // strange exception
            }

            foreach (var moveInformation in possibleMoves)
            {
                var intermediateResult = MakeAnalyzingMoves(moveInformation, depth, playerType, 
                    currentPlayerArmyPositions, otherPlayerArmyPositions);
                //Enemy will lose
                if (double.IsNegativeInfinity(intermediateResult.Item1))
                {
                    resultBenefit = intermediateResult.Item1;
                    bestMoveInformation = moveInformation;
                    break;
                }
                
                if (intermediateResult.Item1 < resultBenefit)
                {
                    resultBenefit = intermediateResult.Item1;
                    bestMoveInformation = moveInformation;
                }
            }
            
            return new Tuple<double, MoveInformation>(resultBenefit, bestMoveInformation);
        }

        private Tuple<double, MoveInformation> MakeAnalyzingMoves(MoveInformation moveInformation, int depth, 
            PlayerType playerType,  List<IntVector2> currentPlayerArmyPositions, 
            List<IntVector2> otherPlayerArmyPositions)
        {
            var memorizedFrom = new ItemAndPosition(GetItemByPosition(moveInformation.From), moveInformation.From);
            var memorizedTo = new ItemAndPosition(GetItemByPosition(moveInformation.To), moveInformation.To);
           
            MakeMove(moveInformation.From, moveInformation.To, false);
            currentPlayerArmyPositions.Remove(moveInformation.From);
            currentPlayerArmyPositions.Remove(moveInformation.To);
            otherPlayerArmyPositions.Remove(moveInformation.From);
            otherPlayerArmyPositions.Remove(moveInformation.To);

            var resultArmy = boardStorageEmulation.GetItem(moveInformation.To.x,
                                                           moveInformation.To.y);

            if (resultArmy.Army != null && resultArmy.Army is UserArmy)
            {
                var userArmy = resultArmy.Army as UserArmy;
                if (playerType == userArmy.playerType)
                {
                    currentPlayerArmyPositions.Add(moveInformation.To);
                }
                else
                {
                    otherPlayerArmyPositions.Add(moveInformation.To);
                }
            }

            var result = AnalyzeStrategy(ChangePlayerType(playerType), false, 
                    depth - 1,otherPlayerArmyPositions, 
                    currentPlayerArmyPositions);

            CancelMove(memorizedFrom, memorizedTo);
            currentPlayerArmyPositions.Remove(moveInformation.To);
            otherPlayerArmyPositions.Remove(moveInformation.To);
            if (memorizedFrom.item?.Army != null)
            {
                if (memorizedFrom.item.Army.playerType == playerType)
                {
                    currentPlayerArmyPositions.Add(moveInformation.From);
                }

                 if (memorizedFrom.item.Army.playerType == ChangePlayerType(playerType))
                 {
                    otherPlayerArmyPositions.Add(moveInformation.From);
                 }
            }

            if (memorizedTo.item?.Army != null)
            {
                if (memorizedTo.item.Army.playerType == playerType)
                {
                    currentPlayerArmyPositions.Add(moveInformation.To);
                }

                if (memorizedTo.item.Army.playerType == ChangePlayerType(playerType))
                {
                    otherPlayerArmyPositions.Add(moveInformation.To);
                }
            }

            return result;
        }

        private void CancelMove(ItemAndPosition from, ItemAndPosition to)
        {
            from.item.Army.SetActive();
            boardStorageEmulation.SetItem(from.x, from.y, from.item);
            boardStorageEmulation.SetItem(to.x, to.y, to.item);
        }

        private ArmyStorageItemEmulation GetItemByPosition(IntVector2 position)
        {
            return boardStorageEmulation.GetItem(position.x, position.y);
        }

        public void MakeMove(IntVector2 from, IntVector2 to, bool isRealMove) //changes boardStorageEmulation
        {
            var fromItem = boardStorageEmulation.GetItem(from.x, from.y);
            var toItem = boardStorageEmulation.GetItem(to.x, to.y);

            if (fromItem == null)
            {
                throw new ArgumentException("cannot move nonexistent army");
            }

            boardStorageEmulation.SetItem(from.x, from.y, null);
            if (toItem?.Army == null)
            {
                boardStorageEmulation.SetItem(to.x, to.y, fromItem);

                if (isRealMove && fromItem.Army is UserArmy)
                {
                    ((UserArmy)fromItem.Army).SetInactive();
                }
            }
            else
            {
                var resultArmy = toItem.Army.PerformAction(fromItem.Army);
                
                if (!isRealMove)
                {
                    resultArmy.SetActive();
                }

                boardStorageEmulation.SetItem(to.x, to.y, new ArmyStorageItemEmulation(resultArmy));
            }
        }

        public void SetArmyInactive(int x, int y)
        {
            var item = boardStorageEmulation.GetItem(x, y);
            if (item?.Army is UserArmy)
            {
                (item.Army as UserArmy).SetInactive();
            }
        }

        private List<MoveInformation> GetListOfMoves(List<IntVector2> playerArmiesPositions, bool isFirstTurn)
        {
            var possibleMoves = new List<MoveInformation>();

            foreach (var position in playerArmiesPositions)
            {
                var army = boardStorageEmulation.GetItem(position.x, position.y).Army;
                if (army is UserArmy)
                {
                    var userArmy = army as UserArmy;
                    if (userArmy.IsActive() || !isFirstTurn)
                    {
                        AddPossibleMoves(possibleMoves, position);
                    }
                }
            }

            return possibleMoves;
        }
        
        private void AddPossibleMoves(List<MoveInformation> possibleMoves, IntVector2 position)
        {
            int x = position.x;
            int y = position.y;

            if (PossibleToMove(x, y - 1))
            {
                possibleMoves.Add(new MoveInformation(position, new IntVector2(x, y - 1)));
            }

            if (PossibleToMove(x - 1, y))
            {
                possibleMoves.Add(new MoveInformation(position, new IntVector2(x - 1, y)));
            }

            if (PossibleToMove(x, y + 1))
            {
                possibleMoves.Add(new MoveInformation(position, new IntVector2(x,y + 1)));
            }

            if (PossibleToMove(x + 1, y))
            {
                possibleMoves.Add(new MoveInformation(position, new IntVector2(x + 1, y)));
            }
        }

        private bool PossibleToMove(int x, int y)
        {
            int boardHeight = boardStorageEmulation.Height;
            int boardWidth = boardStorageEmulation.Width;

            if (x > boardWidth || x <= 0)
            {
                return false;
            }
            
            if (y > boardHeight || y <= 0)
            {
                return false;
            }

            var item = boardStorageEmulation.GetItem(x, y);
            
            return item == null || item.IsAvailable;
        }

        public int GetNumberOfActiveArmies(PlayerType playerType)
        {
            return GetActiveArmies(playerType).Count;
        }

        private List<IntVector2> GetActiveArmies(PlayerType playerType)
        {
            var activeArmiesPositions = new List<IntVector2>();
            
            foreach (var position in FindPlayerArmies()[playerType])
            {
                var army = boardStorageEmulation.GetItem(position.x, position.y).Army;

                if (army is UserArmy)
                {
                    var userArmy = army as UserArmy;
                    if (userArmy.IsActive())
                    {
                        activeArmiesPositions.Add(position);
                    }
                }
                else
                {
                    throw new Exception("something went wrong"); // I don't know what to throw
                }
            }

            return activeArmiesPositions;
        }
    }
}