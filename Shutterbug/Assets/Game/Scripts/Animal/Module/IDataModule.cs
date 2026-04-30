namespace Game.Scripts.Module
{
    public interface IDataModule
    {
        public float GetViewDistance();
        public float GetViewAngle();
        public float GetDistanceForSpecialState();
    }
}