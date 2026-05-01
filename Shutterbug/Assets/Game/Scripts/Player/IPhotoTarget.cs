namespace Game.Scripts
{
    public interface IPhotoTarget
    {
        AnimalType AnimalType { get; set; }
        AnimalState CurrentState { get; }
    }
}