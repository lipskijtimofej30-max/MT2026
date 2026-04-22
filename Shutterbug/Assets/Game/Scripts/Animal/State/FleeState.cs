using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class FleeState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly Animator _animator;
        private readonly float _safeDistance;
        private readonly float _fleeDistance = 8f;
        private readonly int _maxAttempts = 10;
        
        public AnimalState StateType => AnimalState.Flee;
        
        public FleeState(NavMeshAgent agent, GameMath gameMath, Animator animator, float safeDistance)
        {
            _agent = agent;
            _gameMath = gameMath;
            _animator = animator;
            _safeDistance = safeDistance;
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
            
            _animator.SetInteger("AnimIndex", 1);
            _animator.SetTrigger("Next");
            
            _agent.SetDestination(target);
            if (_gameMath.DistanceToPlayer(_agent.transform) < _safeDistance)
                return StateAction.Stay;

            while (!ct.IsCancellationRequested)
            {
                if (_gameMath.DistanceToPlayer(_agent.transform) > _safeDistance)
                    return StateAction.GoToIdle;

                await UniTask.Yield(ct);
            }
            
            return StateAction.Stay;
        }

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
