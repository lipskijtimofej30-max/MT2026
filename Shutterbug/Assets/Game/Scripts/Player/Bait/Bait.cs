using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class Bait : MonoBehaviour
    {
        private BaitRegistry _baitRegistry;

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
}
