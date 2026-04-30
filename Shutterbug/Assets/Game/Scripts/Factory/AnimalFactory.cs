using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Factory
{
    public class AnimalFactory: MonoBehaviour, IAnimalFactory
    {
        
        private DiContainer _container;
        private AnimalRegistry _animalRegistry;
        
        [Inject]
        private void Construct(DiContainer container, AnimalRegistry animalRegistry)
        {
            _container = container;
            _animalRegistry = animalRegistry;
        }

        public BaseAnimalBrain Spawn(Vector3 position, BaseAnimalBrain prefab)
        {
            var obj = _container.InstantiatePrefabForComponent<BaseAnimalBrain>(prefab.gameObject, position, Quaternion.identity, transform);
            _animalRegistry.Register(obj);
            Debug.Log($"Spawn animal: {obj.name}");
            return obj;
        }
    }
}
