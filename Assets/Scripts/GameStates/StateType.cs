namespace Assets.Scripts
{
    /// <summary>
    /// Game states.
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// State of start menu
        /// </summary>
        START_GAME_STATE,
        
        /// <summary>
        /// State of choosing board mode.
        /// </summary>
        CHOOSE_BOARD_GAME_STATE,
        
        /// <summary>
        /// State of searching for opponents.
        /// </summary>
        LOBBY_GAME_STATE,
        
        /// <summary>
        /// State of network play mode.
        /// </summary>
        NETWORK_GAME_STATE,
        
        /// <summary>
        /// State of play mode with bot.
        /// </summary>
        AI_GAME_STATE,
        
        /// <summary>
        /// State of 2-players play mode on one device.
        /// </summary>
        ONE_DEVICE_MULTIPLAYER_STATE,
        
        /// <summary>
        /// State of displaying results.
        /// </summary>
        RESULT_GAME_STATE,
        
        /// <summary>
        /// State of displaying an info message.
        /// </summary>
        INFO_GAME_STATE,
        
        /// <summary>
        /// State to display the tutorial menu.
        /// </summary>
        TUTORIAL_MENU_GAME_STATE,
        
        /// <summary>
        /// State to display the pages with game description.
        /// </summary>
        TUTORIAL_PAGES_GAME_STATE
    }

}