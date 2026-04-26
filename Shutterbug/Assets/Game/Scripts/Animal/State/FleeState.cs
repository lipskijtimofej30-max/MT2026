using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class FleeState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly RabbitAnimatorModule _animatorModule;
        private readonly Func<bool> _conditionMet;
        private readonly float _fleeDistance = 8f;
        private readonly int _maxAttempts = 10;
        
        public AnimalState StateType => AnimalState.Flee;
        
        public FleeState(NavMeshAgent agent, GameMath gameMath, RabbitAnimatorModule animatorModule, Func<bool> conditionMet)
        {
            _agent = agent;
            _gameMath = gameMath;
            _animatorModule = animatorModule;
            _conditionMet = conditionMet;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            Vector3 fleeDir = _gameMath.GetDirectionToPlayer(_agent.transform);
            Vector3 desiredPos = _agent.transform.position + fleeDir * _fleeDistance;
            Vector3 target = GetNavMeshPoint(desiredPos);
            
            if (target == Vector3.zero)
            {
                Debug.LogWarning("FleeState: no valid NavMesh point found, going to Idle");
                return StateAction.GoToIdle;
            }

            _agent.speed = 7f;
            _agent.SetDestination(target);
            
            if (_conditionMet())
                return StateAction.Stay;
            
            if (_agent.remainingDistance > 0.1f)
            {
                _animatorModule.StartAnimation(RabbitAnimatorModule.WALKORFLEE);

                await UniTask.Yield(ct);
            }
           
            while (!ct.IsCancellationRequested && _agent.isActiveAndEnabled)
            {
                if (!_conditionMet())
                    return StateAction.GoToWalk;
                
                await UniTask.Yield(ct);
            }
            return StateAction.Stay;
        }

        public float GetStateMultiplier() => 1.5f; 

        private Vector3 GetNavMeshPoint(Vector3 desiredPosition)
        {
            for (int i = 0; i < _maxAttempts; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 2f; 
                if (NavMesh.SamplePosition(desiredPosition + offset, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return Vector3.zero;
        }
    }
}
