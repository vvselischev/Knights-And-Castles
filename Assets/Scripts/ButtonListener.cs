using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ButtonListener : MonoBehaviour
    {
        // IBonus ActiveBonus;
        public GameIcon activeBonusIcon;
       // public BonusFactory factory;
        public ControllerManager controllerManager;
        public BoardStorage storage;

        public void PutAway()
        {
            Reset();
            //controllerManager.currentController.OnBonusRemoveFromHand();
        }

        public void OnBoardClick(BoardButton boardButton)
        {
            /*if (HasActiveBonus())
            {
                GameInventory inventory = controllerManager.currentController.GetInventory();
                BonusType bonus = ActiveBonus.GetBonusType();
                if (inventory.HasBonus(bonus))
                {
                    ActiveBonus.DropOnBoard(boardButton);
                    if (ActiveBonus.CanHoldInStorage())
                    {
                        if (storage.TryAdd(boardButton, ActiveBonus))
                        {
                            boardButton.icon.SetImage(ActiveBonus.GetBonusImage());
                            inventory.RemoveBonus(bonus);
                        }
                    }

                }
            }*/
            controllerManager.currentController.OnButtonClick(boardButton);
        }

        public void Reset()
        {
            //ActiveBonus = null;
            activeBonusIcon.Reset();
        }
    }
}