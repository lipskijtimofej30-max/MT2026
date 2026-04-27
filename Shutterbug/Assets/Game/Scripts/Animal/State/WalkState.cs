using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class WalkState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly RabbitAnimatorModule _animator;
        private readonly Func<bool> _conditionMet;
        private readonly float _radius;
        private readonly int _maxAttempts = 10;

        public AnimalState StateType => AnimalState.Walk;


        public WalkState(NavMeshAgent agent, RabbitAnimatorModule animator, Func<bool> conditionMet, float radius)
        {
            _agent = agent;
            _animator = animator;
            _conditionMet = conditionMet;
            _radius = radius;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            if (_conditionMet())
            {
                Debug.Log("[WalkState] The player is visible.");
                return StateAction.GoToAlert;
            }
            
            _agent.speed = 3.5f;
            Vector3 target = GetRandomNavMeshPoint(_agent.transform.position, _radius);
            if (target == Vector3.zero)
            {
                return StateAction.GoToIdle;
            }

            _animator.StartAnimation(RabbitAnimatorModule.WALKORFLEE);
            
            _agent.SetDestination(target);

            float stuckTime = 0f;
            Vector3 lastPos = _agent.transform.position;

            while (!ct.IsCancellationRequested && _agent != null && _agent.pathPending == false && _agent.remainingDistance > 0.1f)
            {
                if (_conditionMet())
                {
                    Debug.Log($"[WalkState] Player entered flee radius while walking, fleeing!");
                    return StateAction.GoToAlert;
                }
                
                if (Vector3.Distance(_agent.transform.position, lastPos) < 0.01f)
                {
                    stuckTime += Time.deltaTime;
                    if (stuckTime > 2f)
                        break;
                }
                else
                {
                    stuckTime = 0f;
                    lastPos = _agent.transform.position;
                }

                await UniTask.Yield(ct);
            }
            

            if (Random.value < 0.2f)
                return StateAction.Stay;

            return StateAction.GoToIdle;
        }
        
        public float GetStateMultiplier() => 1.2f; 

        private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
        {
            for (int i = 0; i < _maxAttempts; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * radius;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return Vector3.zero;
        }
    }
}
