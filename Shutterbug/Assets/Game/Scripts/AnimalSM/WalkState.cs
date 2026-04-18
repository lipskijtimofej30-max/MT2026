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
        private readonly float _distToFlee;
        private readonly float _radius;
        private readonly int _maxAttempts = 10; // попыток найти точку на NavMesh

        public WalkState(NavMeshAgent agent, GameMath gameMath, float distToFlee, float radius)
        {
            _agent = agent;
            _gameMath = gameMath;
            _distToFlee = distToFlee;
            _radius = radius;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            Vector3 target = GetRandomNavMeshPoint(_agent.transform.position, _radius);
            if (target == Vector3.zero)
            {
                // Не смогли найти точку на NavMesh — возвращаемся в Idle
                return StateAction.GoToIdle;
            }

            _agent.SetDestination(target);

            float stuckTime = 0f;
            Vector3 lastPos = _agent.transform.position;

            // Ждём, пока агент не достигнет цели или не застрянет
            while (!ct.IsCancellationRequested && _agent.pathPending == false && _agent.remainingDistance > 0.1f)
            {
                // Проверка на зависание
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

            // Проверяем, не нужно ли убегать (игрок близко)
            if (_gameMath.DistanceToPlayer(_agent.transform) < _distToFlee)
                return StateAction.GoToFlee;

            // Случайный шанс остаться в Walk (Stay) или перейти в Idle
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
            // Если не нашли — возвращаем нулевой вектор как признак неудачи
            return Vector3.zero;
        }
    }
}