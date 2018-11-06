using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UserController
    {
        private PlayGameState playGameState;
        private BoardStorage boardStorage;
        private TurnType turnType;

        private Army choosedArmy;
        private Dictionary<Army, Vector3> userArmies;

        public UserController(TurnType turnType, BoardStorage storage, PlayGameState playGameState)
        {
            this.turnType = turnType;
            boardStorage = storage;
            this.playGameState = playGameState;
        }

        public void OnButtonClick(BoardButton boardButton)
        {
            

            if (boardButton.boardX == 1)
            {
                FinishTurn();
            }
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