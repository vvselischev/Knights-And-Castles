using UnityEngine.UI;

namespace Assets.Scripts
{
    public class NetworkInputListener : InputListener
    {
        private MultiplayerController multiplayerController;

        public Text logText;
        
        public void Init()
        {
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnMessageReceived += ProcessNetworkData;
        }

        public void Stop()
        {
            multiplayerController.OnMessageReceived -= ProcessNetworkData;
        }

        //'M' -- move (otherwise ignore this message), followed by one of:
        //'B' -- button click, followed by x and y
        //'F' -- finish turn
        //'S' -- split
        private void ProcessNetworkData(byte[] message)
        {
            if (message[0] != 'M')
            {
                return;
            }

            if (message[1] == 'B')
            {
                int x = message[2];
                int y = message[3];
                logText.text += "Receive:" + x + " " + y + "\n";
                base.ProcessBoardClick(x, y);
            }
            else if (message[1] == 'F')
            {
                logText.text += "Receive finish turn" + "\n";
                base.ProcessFinishTurnClick();
            }
            else if (message[1] == 'S')
            {
                logText.text += "Receive split" + "\n";
                base.ProcessSplitButtonClick();
            }
        }

        public override void ProcessBoardClick(int x, int y)
        {
            byte[] message = {(byte)'M', (byte)'B', (byte) x, (byte) y};
            logText.text += "Send:" + x + " " + y + "\n";
            multiplayerController.SendMessage(message);
            base.ProcessBoardClick(x, y);
        }

        public override void ProcessSplitButtonClick()
        {
            byte[] message = {(byte) 'M', (byte) 'S'};
            logText.text += "Send split" + "\n";
            multiplayerController.SendMessage(message);
            base.ProcessSplitButtonClick();
        }

        public override void ProcessFinishTurnClick()
        {
            byte[] message = {(byte) 'M', (byte) 'F'};
            logText.text += "Send finish turn" + "\n";
            multiplayerController.SendMessage(message);
            base.ProcessFinishTurnClick();
        }
    }
}