using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class WalkState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly Animator _animator;
        private readonly float _distToFlee;
        private readonly float _radius;
        private readonly int _maxAttempts = 10; 

        public WalkState(NavMeshAgent agent, GameMath gameMath, Animator animator,float distToFlee, float radius)
        {
            _agent = agent;
            _gameMath = gameMath;
            _animator = animator;
            _distToFlee = distToFlee;
            _radius = radius;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            if (_gameMath.DistanceToPlayer(_agent.transform) < _distToFlee)
            {
                Debug.Log("[WalkState] Player inside flee radius immediately, fleeing.");
                return StateAction.GoToFlee;
            }
            
            _agent.speed = 3.5f;
            Vector3 target = GetRandomNavMeshPoint(_agent.transform.position, _radius);
            if (target == Vector3.zero)
            {
                return StateAction.GoToIdle;
            }

            _animator.SetInteger("AnimIndex", 1);
            _animator.SetTrigger("Next");
            
            _agent.SetDestination(target);

            float stuckTime = 0f;
            Vector3 lastPos = _agent.transform.position;

            while (!ct.IsCancellationRequested && _agent != null && _agent.pathPending == false && _agent.remainingDistance > 0.1f)
            {
                if (_gameMath.DistanceToPlayer(_agent.transform) < _distToFlee)
                {
                    Debug.Log($"[WalkState] Player entered flee radius while walking, fleeing!");
                    return StateAction.GoToFlee;
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
