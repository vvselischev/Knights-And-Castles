using UnityEngine;

namespace Assets.Scripts
{
    public interface IGameState
    {
        void InvokeState();
        void CloseState();
    }
}