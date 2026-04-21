using Game.Scripts.Factory;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform _posSpawn;
        private IAnimalFactory _animalFactory;

        [Inject]
        private void Construct(IAnimalFactory animalFactory)
        {
            _animalFactory = animalFactory;
        }

        private void Start()
        {
            _animalFactory.Spawn(_posSpawn.position);
        }
    }
}