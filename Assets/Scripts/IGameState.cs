namespace Assets.Scripts
{
    /// <summary>
    /// Represents a game state to invoke and close them in a common way.
    /// </summary>
    public interface IGameState
    {
        void InvokeState();
        void CloseState();
    }
}