using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChangeStateButton : MonoBehaviour
    {
        public StateType nextState;
        public StateManager stateManager;

        public void OnClick()
        {
            stateManager.ChangeState(nextState);
        }
    }
}