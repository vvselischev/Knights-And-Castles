using UnityEngine;

namespace Assets.Scripts
{
    public class ChangeStateButton : MonoBehaviour
    {
        [SerializeField] private StateType nextStateType;

        public void OnClick()
        {
            StateManager.Instance.ChangeState(nextStateType);
        }
    }
}
