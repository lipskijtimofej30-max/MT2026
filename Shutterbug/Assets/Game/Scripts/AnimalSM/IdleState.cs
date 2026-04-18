using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class IdleState : IState
    {
        private float _minTimeIdle, _maxTimeIdle;

        public IdleState(float minTime, float maxTime)
        {
            _minTimeIdle = minTime;
            _maxTimeIdle = maxTime;
        }
        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            var duration = Random.Range(_minTimeIdle, _maxTimeIdle);
            Debug.Log($"Idle state wait {duration} seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
            if (Random.value < 0.3f)
                return StateAction.Stay;

            return StateAction.GoToWalk;
        }
    }
}