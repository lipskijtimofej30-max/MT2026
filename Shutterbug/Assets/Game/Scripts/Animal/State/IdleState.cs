using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class IdleState : IState
    {
        private float _minTimeIdle, _maxTimeIdle;
        private RabbitAnimatorModule _animatorModule;
        private NavMeshAgent _agent;
        private EatingState _eatingState;
        private BaitRegistry _baitRegistry;
        private Func<bool> _conditionMetToAlert;
        private Func<bool> _conditionMetToSpecialState;
        private float _baitCheckCooldown = 0.5f;
        private float _lastBaitCheckTime = 0f;
        private float _smellRadius; 
        public AnimalState StateType => AnimalState.Idle;

        /// <summary>
        /// Передавать метод для перехода в состоние Alert
        /// </summary>
        public IdleState(RabbitAnimatorModule animatorModule,NavMeshAgent agent, 
            Func<bool> conditionMetToAlert, Func<bool> conditionMetToSpecialState,
            RabbitConfig config, BaitRegistry baitRegistry, EatingState eatingState)
        {
            _animatorModule = animatorModule;
            _conditionMetToAlert = conditionMetToAlert;
            _conditionMetToSpecialState = conditionMetToSpecialState;
            _baitRegistry = baitRegistry;
            _agent = agent;
            _eatingState = eatingState;
            _smellRadius = config.SmellRadius;
            _minTimeIdle = config.IdleTime.Min;
            _maxTimeIdle = config.IdleTime.Max;
        }
        
        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            float elapsed = 0f;
            float duration = Random.Range(_minTimeIdle, _maxTimeIdle);
            
            _animatorModule.StartAnimation(RabbitAnimatorModule.IDLE);
            while (elapsed < duration)
            {
                if (_conditionMetToAlert())
                    return StateAction.GoToAlert;
                if(_conditionMetToSpecialState())
                    return StateAction.GoToSpecialState;
                if (Time.time - _lastBaitCheckTime > _baitCheckCooldown)
                {
                    _lastBaitCheckTime = Time.time;
            
                    Bait closestBait = _baitRegistry.GetClosestBait(_agent.transform.position, _smellRadius);
                    if (closestBait != null)
                    {
                        Debug.Log("Почуял приманку!");
                        
                        _eatingState.TargetBait = closestBait; 
                        
                        return StateAction.GoToBaitState;
                    }
                }

                await UniTask.Delay(100, cancellationToken: ct); 
                elapsed += 0.1f;
            }

            if (Random.value < 0.3f)
                return StateAction.Stay;

            return StateAction.GoToWalk;
        }
        
        public float GetStateMultiplier() => 1f; 

    }
}
