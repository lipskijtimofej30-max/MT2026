using System;
using Cysharp.Threading.Tasks;
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

        public PlayerDeathHandler(
            SignalBus signalBus, 
            PlayerController player, 
            IPlayerInventory inventory, 
            DeathUI deathUI,
            [Inject(Id = "RespawnPoint")] Transform respawnPoint) // Инъекция точки по ID из Zenject
        {
            _signalBus = signalBus;
            _player = player;
            _inventory = inventory;
            _deathUI = deathUI;
            _respawnPoint = respawnPoint;
        }

        public void Initialize() => _signalBus.Subscribe<AttackPlayerSignal>(OnPlayerDied);
        public void Dispose() => _signalBus.Unsubscribe<AttackPlayerSignal>(OnPlayerDied);

        private async void OnPlayerDied(AttackPlayerSignal signal)
        {
            _player.enabled = false;

            await _deathUI.FadeOut().ToUniTask();

            _inventory.Clear();

            _player.transform.position = _respawnPoint.position;
            _player.transform.rotation = _respawnPoint.rotation;

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            // Место для будущих диалогов Профессора
            Debug.Log("Профессор: 'Осторожнее, мир за пределами базы опасен...'");

            await _deathUI.FadeIn().ToUniTask();
            _player.enabled = true;
        }
    }
}
