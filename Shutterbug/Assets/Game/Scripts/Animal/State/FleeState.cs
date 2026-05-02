using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class FleeState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly IAnimatorModule _animatorModule;
        
        private readonly float _runPointDistance = 12f; 
        private readonly float _safeDistance;

        public AnimalState StateType => AnimalState.Flee;
        
        public FleeState(NavMeshAgent agent, GameMath gameMath, IAnimatorModule animatorModule, AnimalConfig config)
        {
            _agent = agent;
            _gameMath = gameMath;
            _animatorModule = animatorModule;
            _runPointDistance = config.ToSpecialStateDistance;
            _safeDistance = _runPointDistance + 3f;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            _agent.speed = 10f;
            _agent.angularSpeed = 400f; 
            _agent.acceleration = 50f;
            _animatorModule.StartAnimationSpecialState();

            SetNewFleeDestination();

            while (!ct.IsCancellationRequested)
            {
                float distToPlayer = Vector3.Distance(_agent.transform.position, _gameMath.PlayerPosition);
                
                // 1. Проверка на безопасность: если убежали достаточно далеко
                if (distToPlayer > _safeDistance) 
                {
                    // Ждем, пока добежит до последней заданной точки, чтобы остановка была плавной
                    if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.5f)
                    {
                        Debug.Log("[FleeState] Животное в безопасности, переходим в Idle");
                        return StateAction.GoToIdle;
                    }
                }
                else
                {
                    // 2. Игрок всё ещё близко. Проверяем, добежали ли мы до точки побега
                    if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 1f)
                    {
                        Debug.Log("[FleeState] Точка достигнута, игрок всё ещё рядом. Ищем новую точку.");
                        SetNewFleeDestination();
                    }
                    else
                    {
                        // 3. Защита от перехвата: если мы бежим, но вектор нашего движения направлен В СТОРОНУ игрока
                        Vector3 currentDirToPlayer = (_gameMath.PlayerPosition - _agent.transform.position).normalized;
                        Vector3 currentMoveDir = _agent.velocity.normalized;
                        
                        // Если угол между скоростью и направлением на игрока острый (мы бежим к нему)
                        if (_agent.velocity.sqrMagnitude > 0.1f && Vector3.Dot(currentMoveDir, currentDirToPlayer) > 0.3f)
                        {
                            Debug.Log("[FleeState] Игрок перерезал путь! Экстренно меняем направление.");
                            SetNewFleeDestination();
                        }
                    }
                }
                
                await UniTask.Yield(ct);
            }
            return StateAction.GoToIdle;
        }

        private void SetNewFleeDestination()
        {
            // Строго вектор ОТ игрока на текущий момент (а не старый сохраненный)
            Vector3 fleeDir = (_agent.transform.position - _gameMath.PlayerPosition).normalized;
            fleeDir.y = 0; 

            Vector3 target = GetValidFleePoint(_agent.transform.position + fleeDir * _runPointDistance, fleeDir);
            
            if (target != Vector3.zero) 
            {
                _agent.SetDestination(target);
            }
            else
            {
                // Защита от тупика: Если зажали в углу (прямых точек нет), бежим в случайную сторону
                Vector3 randomDir = Quaternion.Euler(0, UnityEngine.Random.Range(-90f, 90f), 0) * fleeDir;
                Vector3 fallbackTarget = _agent.transform.position + randomDir * (_runPointDistance / 2f);
                if (NavMesh.SamplePosition(fallbackTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                }
            }
        }

        private Vector3 GetValidFleePoint(Vector3 desiredPoint, Vector3 currentFleeDir)
        {
            if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            {
                if (!IsObstacleBetween(_agent.transform.position, hit.position))
                    return hit.position;
            }
            
            // Расширяем поиск углов: пытаемся найти обходные пути, увеличивая угол
            for (int angle = 20; angle <= 120; angle += 20)
            {
                foreach (int side in new int[] { 1, -1 })
                {
                    Vector3 rotatedDir = Quaternion.AngleAxis(angle * side, Vector3.up) * currentFleeDir;
                    Vector3 potentialPos = _agent.transform.position + rotatedDir * _runPointDistance;

                    if (NavMesh.SamplePosition(potentialPos, out hit, 3f, NavMesh.AllAreas))
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
            // КРИТИЧНОЕ ИСПРАВЛЕНИЕ: Поднимаем луч на 0.5f от земли, чтобы не цеплять пол и мелкие кочки!
            Vector3 startPos = from + Vector3.up * 0.5f; 
            Vector3 endPos = to + Vector3.up * 0.5f;
            Vector3 direction = endPos - startPos;
            float distance = direction.magnitude;
            
            if (Physics.Raycast(startPos, direction.normalized, out RaycastHit hit, distance))
            {
                return hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle");
            }
            return false;
        }
                
        public float GetStateMultiplier() => 1.5f; 
    }
}