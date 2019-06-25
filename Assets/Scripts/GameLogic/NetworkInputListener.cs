using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Receives the local input and transfers it to the network.
    /// Receives the network input and transfers it to the current controller.
    /// Inverts received coordinates.
    /// </summary>
    public class NetworkInputListener : InputListener
    {
        private MultiplayerController multiplayerController;
        private int blockWidth;
        private int blockHeight;

        /// <summary>
        /// Must be called before usage.
        /// </summary>
        public void Initialize(ControllerManager controllerManager, int blockWidth, int blockHeight)
        {
            base.Initialize(controllerManager);
            this.blockWidth = blockWidth;
            this.blockHeight = blockHeight;
            multiplayerController = MultiplayerController.GetInstance();
            multiplayerController.OnMessageReceived += ProcessNetworkData;
        }

        public void Stop()
        {
            multiplayerController.OnMessageReceived -= ProcessNetworkData;
        }
        
        /// <summary>
        /// Processes incoming messages by the given convention:
        /// 'M' -- move (otherwise ignore this message), followed by one of:
        /// 'B' -- button click, followed by x and y
        /// 'F' -- finish turn
        /// 'S' -- split
        /// </summary>
        private void ProcessNetworkData(byte[] message)
        {
            if (message[0] != 'M')
            {
                return;
            }

            if (message[1] == 'B')
            {
                var x = blockWidth - message[2] + 1;
                var y = blockHeight - message[3] + 1;
                base.ProcessBoardClick(x, y);
            }
            else if (message[1] == 'F')
            {
                base.ProcessFinishTurnClick();
            }
            else if (message[1] == 'S')
            {
                base.ProcessSplitButtonClick();
            }
        }

        public override void ProcessBoardClick(int x, int y)
        {
            byte[] message = {(byte)'M', (byte)'B', (byte) x, (byte) y};
            multiplayerController.SendMessage(message);
            base.ProcessBoardClick(x, y);
        }

        public override void ProcessSplitButtonClick()
        {
            byte[] message = {(byte) 'M', (byte) 'S'};
            multiplayerController.SendMessage(message);
            base.ProcessSplitButtonClick();
        }

        public override void ProcessFinishTurnClick()
        {
            byte[] message = {(byte) 'M', (byte) 'F'};
            multiplayerController.SendMessage(message);
            base.ProcessFinishTurnClick();
        }
    }
}