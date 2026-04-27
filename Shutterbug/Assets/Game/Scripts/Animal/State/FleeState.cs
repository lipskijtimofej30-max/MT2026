using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class FleeState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly RabbitAnimatorModule _animatorModule;
        private readonly Func<bool> _conditionMet;
        private readonly float _fleeDistance = 3f;
        private readonly int _maxAttempts = 20;
        
        public AnimalState StateType => AnimalState.Flee;
        
        /// <summary>
        /// Передавать метод для перехода в состоние Alert
        /// </summary>
        public FleeState(NavMeshAgent agent, GameMath gameMath, RabbitAnimatorModule animatorModule, Func<bool> conditionMet)
        {
            _agent = agent;
            _gameMath = gameMath;
            _animatorModule = animatorModule;
            _conditionMet = conditionMet;
        }

       public async UniTask<StateAction> OnEnter(CancellationToken ct)
{
        Vector3 toPlayer = _gameMath.GetDirectionToPlayer(_agent.transform);
        Vector3 fleeDir = toPlayer.normalized;

        _agent.speed = 10f;
        _animatorModule.StartAnimation(RabbitAnimatorModule.WALKORFLEE);

        Vector3 target = GetValidFleePoint(_agent.transform.position + fleeDir * _fleeDistance, fleeDir);
        if (target != Vector3.zero) _agent.SetDestination(target);

        while (!ct.IsCancellationRequested)
        {
            float distToPlayer = Vector3.Distance(_agent.transform.position, _gameMath.PlayerPosition);
            
            if (distToPlayer > _fleeDistance*2) 
            {
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    Debug.Log("Заяц в безопасности, переходим в Idle");
                    return StateAction.GoToIdle;
                }
            }

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (distToPlayer < 6f)
                {
                    Debug.Log("Точка достигнута, но игрок близко. Ищем новую точку.");
                    Vector3 nextTarget = GetValidFleePoint(_agent.transform.position + fleeDir * _fleeDistance, fleeDir);
                    if (nextTarget != Vector3.zero) _agent.SetDestination(nextTarget);
                }
            }
            await UniTask.Yield(ct);
        }
        return StateAction.GoToIdle;
    }
        private Vector3 GetValidFleePoint(Vector3 desiredPoint, Vector3 currentFleeDir)
        {
            if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit hit, 3.5f, NavMesh.AllAreas))
            {
                if (!IsObstacleBetween(_agent.transform.position, hit.position))
                    return hit.position;
            }
            
            for (int angle = 15; angle <= 90; angle += 15)
            {
                foreach (int side in new int[] { 1, -1 })
                {
                    Vector3 rotatedDir = Quaternion.AngleAxis(angle * side, Vector3.up) * currentFleeDir;
                    Vector3 potentialPos = _agent.transform.position + rotatedDir * _fleeDistance;

                    if (NavMesh.SamplePosition(potentialPos, out hit, 2f, NavMesh.AllAreas))
                    {
                        if (!IsObstacleBetween(_agent.transform.position, hit.position))
                        {
                            return hit.position;
                        }
                    }
                }
            }
    
            return Vector3.zero;
        }

        private bool IsObstacleBetween(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            float distance = direction.magnitude;
            if (Physics.Raycast(from, direction.normalized, out RaycastHit hit, distance))
            {
                return hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle");
            }
            return false;
        }
                
        public float GetStateMultiplier() => 1.5f; 
    }
}
