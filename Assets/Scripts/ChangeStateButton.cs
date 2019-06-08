using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Simple button that switches the game to the corresponding state.
    /// This state must be set in the editor.
    /// </summary>
    public class ChangeStateButton : MonoBehaviour
    {
        [SerializeField] private StateType nextStateType;

        public void OnClick()
        {
            StateManager.Instance.ChangeState(nextStateType);
        }
    }
}
