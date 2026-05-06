using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.UI;
using Game.Signals;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class PlayerDeathHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly PlayerController _player;
        private readonly IPlayerInventory _inventory;
        private readonly DeathUI _deathUI;
        private readonly Transform _respawnPoint;

        private bool _isProcessingDeath;

        public PlayerDeathHandler(
            SignalBus signalBus, 
            PlayerController player, 
            IPlayerInventory inventory, 
            DeathUI deathUI,
            [Inject(Id = "RespawnPoint")] Transform respawnPoint)
        {
            _signalBus = signalBus;
            _player = player;
            _inventory = inventory;
            _deathUI = deathUI;
            _respawnPoint = respawnPoint;
        }

        public void Initialize() => _signalBus.Subscribe<AttackPlayerSignal>(HandlePlayerDeath);
        public void Dispose() => _signalBus.Unsubscribe<AttackPlayerSignal>(HandlePlayerDeath);

        private void HandlePlayerDeath(AttackPlayerSignal signal)
        {
            OnPlayerDiedAsync(signal).Forget(); 
        }

        private async UniTaskVoid OnPlayerDiedAsync(AttackPlayerSignal signal)
        {
            if (_isProcessingDeath) return;
            _isProcessingDeath = true;

            try 
            {
                _player.enabled = false;

                await _deathUI.FadeOut().AsyncWaitForCompletion();
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
                
                _inventory.Clear();
                _player.transform.position = _respawnPoint.position;
                _player.transform.rotation = _respawnPoint.rotation;

                await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

                Debug.Log("Профессор: 'Осторожнее...'");

                await _deathUI.FadeIn().AsyncWaitForCompletion();
                _player.enabled = true;
            }
            finally 
            {
                _isProcessingDeath = false;
            }
        }
    }
}
