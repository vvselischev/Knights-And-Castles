namespace Assets.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// Calls split button process method on the listener.
    /// </summary>
    public class SplitButton : LockableButton
    {
        public override void OnClick()
        {
            if (enabled)
            {
                InputListener.ProcessSplitButtonClick();
            }
        }
    }
}