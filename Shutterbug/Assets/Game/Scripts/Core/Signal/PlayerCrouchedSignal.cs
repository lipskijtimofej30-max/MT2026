namespace Game.Signals
{
    public class PlayerCrouchedSignal
    {
        public bool IsCrouched { get; }

        public PlayerCrouchedSignal(bool isCrouched)
        {
            IsCrouched = isCrouched;
        }
    }
}

