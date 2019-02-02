using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class OneDeviceMultiplayerGameState : PlayGameState
    {
        public override void InvokeState()
        {
            base.InvokeState();
            board.EnableBoard();
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            storage.InvertBoard();
        }
    }
}