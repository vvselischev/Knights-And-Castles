using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Displays effects during the game.
    /// Enables frames of appropriate colors.
    /// Displays the split mode.
    /// Can be enabled or disabled.
    /// </summary>
    public class RoundEffects : MonoBehaviour
    {
        [SerializeField] private CheckeredButtonBoard board;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private SplitButton splitButton;
        [SerializeField] private Color playerActiveArmyFrameColor;
        [SerializeField] private Color playerInactiveArmyFrameColor;
        [SerializeField] private Color enemyArmyFrameColor;
        [SerializeField] private Color neutralAggressiveArmyFrameColor;
        [SerializeField] private Color neutralFriendlyArmyFrameColor;
        [SerializeField] private Color possibleSplitPositionFrameColor;

        private bool isEnabled;
        private IBoardStorage boardStorage;
        private IntVector2 chosenArmyFramePosition;
        private List<IntVector2> possibleSplitPositions = new List<IntVector2>();

        public void Initialize(IBoardStorage boardStorage)
        {
            this.boardStorage = boardStorage;
        }
        
        public void DisablePlayerEffects()
        {
            isEnabled = false;
        }

        public void EnablePlayerEffects()
        {
            isEnabled = true;
        }

        public void DisableAllFrames()
        {
            foreach (var boardButton in board.GetBoardButtons())
            {
                boardButton.DisableFrame();
            }
        }

        public void EnableSplitMode(List<IntVector2> possiblePositions)
        {
            if (!isEnabled)
            {
                return;
            }
            
            splitButton.EnableFrame();
            possibleSplitPositions = possiblePositions;
            foreach (var position in possibleSplitPositions)
            {
                board.GetBoardButton(position).EnableFrame(possibleSplitPositionFrameColor);
            }
        }
        
        public void DisableSplitMode()
        {
            if (!isEnabled)
            {
                return;
            }
            
            splitButton.DisableFrame();
            foreach (var position in possibleSplitPositions)
            {
                board.GetBoardButton(position).DisableFrame();
            }
            possibleSplitPositions.Clear();
        }

        /// <summary>
        /// Enables the frame across the given position and deactivates the current frame (if exists).
        /// If null is given as position, just turns off the current frame.
        /// </summary>
        public void SetChosenArmyFrame(IntVector2 position)
        {
            if (!isEnabled)
            {
                return;
            }

            if (chosenArmyFramePosition != null)
            {
                DisableFrame(chosenArmyFramePosition);
            }

            if (position != null)
            {
                EnableFrame(position);
            }
            chosenArmyFramePosition = position;
        }
        
        /// <summary>
        /// Disables frame on the given position.
        /// </summary>
        public void DisableFrame(IntVector2 position)
        {
            board.GetBoardButton(position).DisableFrame();
        }
        
        /// <summary>
        /// Enables the appropriate frame depending on the given position on board.
        /// </summary>
        public void EnableFrame(IntVector2 position)
        {
            if (!isEnabled)
            {
                return;
            }
            
            var item = boardStorage.GetItem(position);
            if (!(item is ArmyStorageItem armyItem))
            {
                //We light on only armies.
                return;
            }

            if (!GetArmyColor(armyItem, out var color))
            {
                return;
            }

            board.GetBoardButton(position).EnableFrame(color);
        }

        private bool GetArmyColor(ArmyStorageItem armyItem, out Color color)
        {
            color = Color.clear;
            if (armyItem.Army is UserArmy userArmy)
            {
                color = GetUserArmyFrameColor(userArmy);
            }
            else if (armyItem.Army is NeutralFriendlyArmy)
            {
                color = neutralFriendlyArmyFrameColor;
            }
            else if (armyItem.Army is NeutralAggressiveArmy)
            {
                color = neutralAggressiveArmyFrameColor;
            }
            else
            {
                //Something strange on this cell.
                return false;
            }

            return true;
        }

        private Color GetUserArmyFrameColor(UserArmy userArmy)
        {
            Color color;
            //It is user army.
            if (userArmy.PlayerType == turnManager.GetCurrentPlayerType())
            {
                //It is our army.
                if (userArmy.IsActive())
                {
                    //We can move this army
                    color = playerActiveArmyFrameColor;
                }
                else
                {
                    //We cannot move this army
                    color = playerInactiveArmyFrameColor;
                }
            }
            else
            {
                //It is opponents' army.
                color = enemyArmyFrameColor;
            }

            return color;
        }
    }
}