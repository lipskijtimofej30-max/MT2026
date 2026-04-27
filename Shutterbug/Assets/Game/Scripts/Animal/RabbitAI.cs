using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    [RequireComponent(typeof(NavMeshAgent), typeof(RabbitLookAt))]
    public class RabbitAI : BaseAnimalAI
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimalType _animalType = AnimalType.Rabbit;
        [SerializeField] private float _distanceToFlee = 4f;
        [SerializeField] private float viewAngle = 120f;
        [SerializeField] private float eyeHeight = 0.5f;
        [SerializeField] private float viewDistance = 10f;
        [SerializeField] private LayerMask _obstacleLayer;

        private PlayerController _playerController;
        private GameMath _gameMath;
        private RabbitLookAt _lookAt;
        
        private RabbitAnimatorModule _animatorModule;
        private IdleState _idleState;
        private WalkState _walkState;
        private FleeState _fleeState;
        private AlertState _alertState;

        [Inject]
        private void Construct(GameMath gameMath, PlayerController playerController)
        {
            _gameMath = gameMath;
            _playerController = playerController;
        }
        
        private async UniTaskVoid Start()
        {
            _lookAt = GetComponent<RabbitLookAt>();
            _animatorModule = new RabbitAnimatorModule(_animator);
            
            _idleState = new IdleState(_animatorModule, CanSeePlayer,ShouldFlee,0.3f, 1f);
            _walkState = new WalkState(_agent, _animatorModule, CanSeePlayer, 5f);
            _fleeState = new FleeState(_agent, _gameMath, _animatorModule, ShouldFlee);
            _alertState = new AlertState(_animatorModule, _lookAt, ShouldFlee, 3f,5f);

            Dictionary<StateAction, IState> map = new Dictionary<StateAction, IState>
            {
                { StateAction.GoToIdle, _idleState },
                { StateAction.GoToWalk, _walkState },
                { StateAction.GoToSpecialState, _fleeState },
                {StateAction.GoToAlert, _alertState }
            };
            
            _stateMachine = new StateMachine(map);
            AnimalType = _animalType;
            await StateMachine.Start(_idleState).AttachExternalCancellation(destroyCancellationToken);
        }

        private bool ShouldFlee()
        { 
            return _gameMath.DistanceToPlayer(transform) < _distanceToFlee;
        }

        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }
        
        bool CanSeePlayer()
        {
            if (_playerController == null) return false;

            Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
            Vector3 playerPos = _playerController.transform.position + Vector3.up * 1f; // грудь игрока
            Vector3 direction = playerPos - eyePos;

            // Квадрат расстояния
            float sqrDist = direction.sqrMagnitude;
            if (sqrDist > viewDistance * viewDistance) return false;

            // Нормализуем после проверки расстояния
            direction.Normalize();

            // Проверка угла через скалярное произведение
            float halfViewCos = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(transform.forward, direction) < halfViewCos) return false;

            // Проверка препятствий
            if (Physics.Linecast(eyePos, playerPos, _obstacleLayer))
                return false;

            return true;
        }

    }
}
