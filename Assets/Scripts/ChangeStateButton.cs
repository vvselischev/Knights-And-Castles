using UnityEngine;

namespace Assets.Scripts
{
    public class ChangeStateButton : MonoBehaviour
    {
        public StateType nextStateType;

        public void OnClick()
        {
            StateManager.Instance.ChangeState(nextStateType);
        }
    }
}
