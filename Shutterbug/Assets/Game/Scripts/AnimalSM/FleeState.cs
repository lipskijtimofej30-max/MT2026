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
        private readonly float _safeDistance;
        private readonly float _fleeDistance = 8f; // как далеко убегать
        private readonly int _maxAttempts = 10;

        public FleeState(NavMeshAgent agent, GameMath gameMath, float safeDistance)
        {
            _agent = agent;
            _gameMath = gameMath;
            _safeDistance = safeDistance;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            // Направление от игрока
            Vector3 fleeDir = _gameMath.GetDirectionToPlayer(_agent.transform);
            Vector3 desiredPos = _agent.transform.position + fleeDir * _fleeDistance;

            Vector3 target = GetNavMeshPoint(desiredPos);
            if (target == Vector3.zero)
            {
                // Не удалось найти точку — возвращаемся в Idle
                return StateAction.GoToIdle;
            }

            _agent.SetDestination(target);

            // Ждём, пока убежим на безопасное расстояние
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
                Vector3 offset = Random.insideUnitSphere * 2f; // небольшой разброс
                if (NavMesh.SamplePosition(desiredPosition + offset, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return Vector3.zero;
        }
    }
}