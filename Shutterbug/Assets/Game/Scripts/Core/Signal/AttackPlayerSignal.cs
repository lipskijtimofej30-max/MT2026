using Game.Scripts;

namespace Game.Signals
{
    public class AttackPlayerSignal
    {
        public PlayerController Player { get; }

        public AttackPlayerSignal(PlayerController player)
        {
            Player = player;
        }
    }
}