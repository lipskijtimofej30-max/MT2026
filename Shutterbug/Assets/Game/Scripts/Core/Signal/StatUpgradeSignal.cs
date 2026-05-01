using Game.Scripts.UpgradeSystem;

namespace Game.Signals
{
    public class StatUpgradeSignal
    {
        public UpgradeType Type { get;}
        public float Value { get;}

        public StatUpgradeSignal(UpgradeType type, float value)
        {
            Type = type;
            Value = value;
        }
    }
}