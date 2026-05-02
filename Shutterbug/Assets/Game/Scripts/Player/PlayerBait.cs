using System.Collections.Generic;
using Game.Scripts;
using Game.Scripts.Data;
using UnityEngine;
using Zenject;

public class PlayerBait : MonoBehaviour
{
    [SerializeField] private Transform _transformReference;
    [SerializeField] private float _throwForce = 5f;
    
    [Header("Bait Settings")]
    [SerializeField] private List<BaitItem> _baitTypes; 
    [SerializeField] private float _throwCooldown = 2f;

    private int _currentBaitIndex = 0;
    private float _currentCooldownTimer = 0f;

    private BaitRegistry _baitRegistry; 
    private IPlayerInventory _inventory;
    private DiContainer _container;

    public BaitItem SelectedBait => _baitTypes.Count > 0 ? _baitTypes[_currentBaitIndex] : null;
    
    public bool IsReady => _currentCooldownTimer <= 0;

    [Inject]
    private void Construct(BaitRegistry baitRegistry, DiContainer container, IPlayerInventory inventory)
    {
        _baitRegistry = baitRegistry;
        _container = container;
        _inventory = inventory;
    }

    private void Update()
    {
        if (_currentCooldownTimer > 0)
        {
            _currentCooldownTimer -= Time.deltaTime;
        }

        HandleSelection();

        if (Input.GetKeyDown(KeyCode.F) && IsReady)
        {
            TryThrowBait();
        }
    }

    private void HandleSelection()
    {
        if (_baitTypes.Count == 0) return;
        
        // Быстрый выбор 1-9
        for (int i = 0; i < _baitTypes.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _currentBaitIndex = i;
            }
        }
    }
    
    private void TryThrowBait()
    {
        var item = SelectedBait;
        if (item == null) return;

        if (_inventory.Has(item, 1))
        {
            _inventory.RemoveItem(item, 1);
            SpawnBait(item);
            
            _currentCooldownTimer = _throwCooldown; 
        }
        else
        {
            //можно вызывать событие для оповещение того что приманки нету и где то вызывать звук
            Debug.LogWarning($"Приманка {item.Name} закончилась!");
        }
    }

    private void SpawnBait(BaitItem data)
    {
        // Проверяем, назначен ли префаб в ScriptableObject
        if (data.BaitPrefab == null)
        {
            Debug.LogError($"У предмета {data.Name} не назначен WorldPrefab!");
            return;
        }

        // Спавним именно тот префаб, который привязан к выбранному предмету
        var bait = _container.InstantiatePrefabForComponent<Bait>(
            data.BaitPrefab, 
            _transformReference.position + _transformReference.forward, 
            Quaternion.identity, 
            null);
        
        _baitRegistry.Register(bait);
        
        if (bait.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(_transformReference.forward * _throwForce, ForceMode.Impulse);
        }
    }
}
