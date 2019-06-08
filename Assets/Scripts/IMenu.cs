namespace Assets.Scripts
{
    /// <summary>
    /// An interface for activating and deactivating menus in a common way.
    /// Menus are supposed to be attached to corresponding canvases that are set active or inactive.
    /// </summary>
    public interface IMenu
    {
        void Activate();
        void Deactivate();
    }
}