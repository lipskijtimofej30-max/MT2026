using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class EatingState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly RabbitAnimatorModule _animator;
        private readonly BaitRegistry _baitRegistry;
        private readonly Func<bool> _conditionMetToAlert;
        
        public AnimalState StateType => AnimalState.Eating;

        public Bait TargetBait { get; set; }

        public EatingState(NavMeshAgent agent, RabbitAnimatorModule animator, BaitRegistry baitRegistry, Func<bool> conditionMetToAlert)
        {
            _agent = agent;
            _animator = animator;
            _baitRegistry = baitRegistry;
            _conditionMetToAlert = conditionMetToAlert;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            if (TargetBait == null) return StateAction.GoToIdle;

            _agent.speed = 4f; 
            _animator.StartAnimation(RabbitAnimatorModule.WALKORFLEE);
            
            Vector3 lastKnownBaitPos = TargetBait.transform.position;
            _agent.SetDestination(lastKnownBaitPos);

            while (!ct.IsCancellationRequested)
            {
                if (_conditionMetToAlert()) return StateAction.GoToAlert;
                if (TargetBait == null) return StateAction.GoToIdle;

                if (!_agent.pathPending && _agent.remainingDistance <= 1.5f)
                {
                    _agent.ResetPath();
                    _animator.StartAnimation(RabbitAnimatorModule.IDLE);
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: ct);
                    
                    if (TargetBait != null) TargetBait.Consume();
                    
                    return StateAction.GoToIdle;
                }

                // ОПТИМИЗАЦИЯ: Обновляем путь только если приманка укатилась больше чем на 0.5 метра
                if (Vector3.Distance(TargetBait.transform.position, lastKnownBaitPos) > 0.5f)
                {
                    lastKnownBaitPos = TargetBait.transform.position;
                    _agent.SetDestination(lastKnownBaitPos);
                }

                await UniTask.Yield(ct);
            }

            return StateAction.GoToIdle;
        }
        
        public float GetStateMultiplier() => 1.25f;
    }
}
