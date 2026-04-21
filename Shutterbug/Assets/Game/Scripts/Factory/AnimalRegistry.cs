using System.Collections.Generic;

namespace Game.Scripts.Factory
{
    public class AnimalRegistry
    {
        private List<BaseAnimalAI> _animals = new();
        public IReadOnlyList<BaseAnimalAI> Animals => _animals;
        
        public void Register(BaseAnimalAI animal) => _animals.Add(animal);
        public void Unregister(BaseAnimalAI animal) => _animals.Remove(animal);
    }
}