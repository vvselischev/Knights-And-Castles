using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkPlayGameState : PlayGameState
    {
        public Text logText;
        public override void InvokeState()
        {
            //this should be done in separate method since we need to wait for the connection, opponent etc.
            base.InvokeState();
            MultiplayerController.GetInstance().logText = logText;
            MultiplayerController.GetInstance().SignInAndStartMPGame();
            //MultiplayerController.GetInstance().TrySilentSignIn();
        }
        public override void ChangeTurn()
        {
            base.ChangeTurn();

        }
    }
}