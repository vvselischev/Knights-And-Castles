namespace Assets.Scripts
{
    /// <summary>
    /// Represents a game state to invoke and close them in a common way.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Method is called when the state is invoked.
        /// </summary>
        void InvokeState();
        /// <summary>
        /// Method is called when the state becomes closed.
        /// </summary>
        void CloseState();
    }
}