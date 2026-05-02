using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

public class RabbitLookAt : MonoBehaviour
{
    [SerializeField] private Rig lookAtRig; 
    [SerializeField] private Transform lookAtTarget; 
    [SerializeField] private float transitionSpeed = 2f;
    
    private Transform playerTransform;
    private PlayerController _playerController;
    private CancellationTokenSource _lookCts;

    [Inject]
    private void Construct(PlayerController playerController)
    {
        _playerController = playerController;
    }

    private void Start()
    {
        playerTransform = _playerController.transform;
        lookAtRig.weight = 0;
    }

    public async UniTask StartLooking(CancellationToken ct)
    {
        _lookCts?.Cancel();
        _lookCts?.Dispose();
        _lookCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var linkedCt = _lookCts.Token;

        lookAtRig.weight = 0f;
        
        try 
        {
            while (!linkedCt.IsCancellationRequested)
            {
                lookAtTarget.position = Vector3.Lerp(lookAtTarget.position, playerTransform.position + Vector3.up * 1.5f, Time.deltaTime * 5f);
                lookAtRig.weight = Mathf.MoveTowards(lookAtRig.weight, 1f, Time.deltaTime * transitionSpeed);
                
                await UniTask.Yield(PlayerLoopTiming.Update, linkedCt);
            }
        }
        catch (OperationCanceledException)
        {
            // Глушим ошибку отмены задачи, чтобы она не сыпалась в консоль
        }
    }

    public async UniTask StopLooking(CancellationToken ct)
    {
        _lookCts?.Cancel();
        _lookCts?.Dispose();
        _lookCts = null;

        while (lookAtRig.weight > 0)
        {
            lookAtRig.weight = Mathf.MoveTowards(lookAtRig.weight, 0f, Time.deltaTime * transitionSpeed * 2f);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }
    
    private void OnDestroy()
    {
        _lookCts?.Cancel();
        _lookCts?.Dispose();
    }
}