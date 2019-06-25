namespace Assets.Scripts
{
    /// <summary>
    /// Receives the local input and transfers it to the network.
    /// Receives the network input and transfers it to the current controller.
    /// Inverts received coordinates.
    /// </summary>
    public class NetworkInputListener : InputListener
    {
        /// <summary>
        /// Bytes for message convention.
        /// </summary>
        private const byte splitButtonClickByte = (byte) 'S';
        private const byte finishTurnButtonClickByte = (byte) 'F';
        private const byte boardClickByte = (byte) 'B';
        private const byte moveByte = (byte) 'M';
        
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

        /// <summary>
        /// Stops the listener. No input from the network is processed.
        /// </summary>
        public void Stop()
        {
            multiplayerController.OnMessageReceived -= ProcessNetworkData;
        }
        
        /// <summary>
        /// Processes incoming messages by the given convention:
        /// move byte (otherwise ignore this message), followed by one of:
        /// - board click byte, followed by x and y
        /// - finish turn byte
        /// - split byte
        /// In the case of button click inverts received coordinates.
        /// </summary>
        private void ProcessNetworkData(byte[] message)
        {
            if (message[0] != moveByte)
            {
                return;
            }

            if (message[1] == boardClickByte)
            {
                var x = blockWidth - message[2] + 1;
                var y = blockHeight - message[3] + 1;
                base.ProcessBoardClick(x, y);
            }
            else if (message[1] == finishTurnButtonClickByte)
            {
                base.ProcessFinishTurnClick();
            }
            else if (message[1] == splitButtonClickByte)
            {
                base.ProcessSplitButtonClick();
            }
        }

        /// <summary>
        /// Transfers the board click to the network and to the base listener (for the current player).
        /// </summary>
        public override void ProcessBoardClick(int x, int y)
        {
            byte[] message = {moveByte, boardClickByte, (byte) x, (byte) y};
            multiplayerController.SendMessage(message);
            base.ProcessBoardClick(x, y);
        }

        /// <summary>
        /// Transfers the split button click to the network and to the base listener (for the current player).
        /// </summary>
        public override void ProcessSplitButtonClick()
        {
            byte[] message = {moveByte, splitButtonClickByte};
            multiplayerController.SendMessage(message);
            base.ProcessSplitButtonClick();
        }

        /// <summary>
        /// Transfers the finish turn click to the network and to the base listener (for the current player).
        /// </summary>
        public override void ProcessFinishTurnClick()
        {
            byte[] message = {moveByte, finishTurnButtonClickByte};
            multiplayerController.SendMessage(message);
            base.ProcessFinishTurnClick();
        }
    }
}
