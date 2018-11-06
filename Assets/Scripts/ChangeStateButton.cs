using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChangeStateButton : MonoBehaviour
    {
        public StateType NextState;
        public StateManager stateManager;

        public void OnClick()
        {
            stateManager.ChangeState(NextState);
        }
    }
}