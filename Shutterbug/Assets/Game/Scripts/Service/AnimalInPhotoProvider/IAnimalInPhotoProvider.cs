using System;

namespace Game.Scripts.Service
{
    public interface IAnimalInPhotoProvider
    {
        BaseAnimalAI TargetAnimal { get; set; }
        event Action<BaseAnimalAI> OnTargetAnimalChanged;
    }
}