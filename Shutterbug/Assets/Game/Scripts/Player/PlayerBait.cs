using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using UnityEngine;
using Zenject;

public class PlayerBait : MonoBehaviour
{
    [SerializeField] private Transform _transformReference;
    [SerializeField] private float _maxTimer = 10f;
    [SerializeField] private float _throwForce = 5f;

    private Bait _baitPrefab;
    private float _time = 0f;
    private bool _isReady => _time <= 0f;

    private BaitRegistry _baitRegistry; 
    private DiContainer _container;

    [Inject]
    private void Construct(BaitRegistry baitRegistry,  DiContainer container)
    {
        _baitRegistry = baitRegistry;
        _container = container;
        _baitPrefab = Resources.Load<Bait>("Prefabs/Bait");
        _time = 0f;
    }

    private void Update()
    {
        if (_time > 0) _time -= Time.deltaTime;

        if (_isReady && Input.GetKeyDown(KeyCode.E))
        {
            SpawnBait();
            _time = _maxTimer;
        }
    }

    private void SpawnBait()
    {
        if (_baitPrefab == null) return;
        
        // Спавним в корень сцены (parent: null), чтобы она не летала за игроком
        var bait = _container.InstantiatePrefabForComponent<Bait>(_baitPrefab, _transformReference.position + _transformReference.forward, Quaternion.identity, null);
        _baitRegistry.Register(bait);
        
        if (bait.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(_transformReference.forward * _throwForce, ForceMode.Impulse);
        }
    }
}
