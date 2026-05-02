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
    public class WalkState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly IAnimatorModule _animator;
        private readonly BaitRegistry _baitRegistry;
        private readonly EatingState _eatingState;
        private readonly AnimalConfig _config;
        private readonly Func<bool> _conditionMet;
        private readonly int _maxAttempts = 10;
        private float _baitCheckCooldown = 0.5f;
        private float _lastBaitCheckTime = 0f;

        public AnimalState StateType => AnimalState.Walk;


        public WalkState(NavMeshAgent agent, IAnimatorModule animator, BaitRegistry baitRegistry,
            Func<bool> conditionMet, AnimalConfig config, EatingState eatingState)
        {
            _agent = agent;
            _animator = animator;
            _conditionMet = conditionMet;
            _baitRegistry = baitRegistry;
            _config = config;
            _eatingState = eatingState;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            if (_conditionMet())
            {
                Debug.Log("[WalkState] The player is visible.");
                return StateAction.GoToAlert;
            }
            _agent.speed = 3.5f;
            Vector3 target = GetRandomNavMeshPoint(_agent.transform.position, _config.WalkRadius);
            if (target == Vector3.zero)
            {
                return StateAction.GoToIdle;
            }

            _animator.StartAnimationWalk();
            
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
                
                if (Time.time - _lastBaitCheckTime > _baitCheckCooldown)
                {
                    _lastBaitCheckTime = Time.time;
            
                    Bait closestBait = _baitRegistry.GetClosestBait(_agent.transform.position, _config.SmellRadius, _config.BaitType);
                    if (closestBait != null && closestBait.BaitType == _config.BaitType)
                    {
                        Debug.Log("Почуял приманку!");
                        
                        _eatingState.TargetBait = closestBait; 
                        
                        return StateAction.GoToBaitState;
                    }
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
