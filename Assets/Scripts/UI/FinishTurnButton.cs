using UnityEngine;

namespace Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Calls finish turn button process method on the listener.
    /// </summary>
    public class FinishTurnButton : LockableButton
    {

        public override void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessFinishTurnClick();
            }
        }
    }
}