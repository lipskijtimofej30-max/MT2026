using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class Bait : MonoBehaviour
    {
        [SerializeField] private BaitType _baitType;
        private BaitRegistry _baitRegistry;
        public BaitType BaitType => _baitType;

        [Inject]
        private void Construct(BaitRegistry baitRegistry)
        {
            _baitRegistry = baitRegistry;
        }

        private void OnDisable()    
        {
            _baitRegistry?.Unregister(this);
        }

        public void Consume()
        {
            Destroy(gameObject);
        }
    }

    public enum BaitType
    {
        PlantBait,
        MeatBait
    }
}
