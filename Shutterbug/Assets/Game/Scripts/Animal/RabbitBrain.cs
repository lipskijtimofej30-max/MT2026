using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using Game.Signals;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    [RequireComponent(typeof(NavMeshAgent), typeof(RabbitLookAt))]
    public class RabbitBrain : BaseAnimalBrain
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimalType _animalType = AnimalType.Rabbit;
        [SerializeField] private RabbitData _dataBase;
        [SerializeField] private RabbitData _dataCrouched;
        [SerializeField] private LayerMask _obstacleLayer;

        private RabbitData _data;
        private PlayerController _playerController;
        private SignalBus _signalBus;
        private GameMath _gameMath;
        private RabbitLookAt _lookAt;
        
        private RabbitAnimatorModule _animatorModule;
        private IdleState _idleState;
        private WalkState _walkState;
        private FleeState _fleeState;
        private AlertState _alertState;

        [Inject]
        private void Construct(GameMath gameMath, PlayerController playerController, SignalBus signalBus)
        {
            _gameMath = gameMath;
            _playerController = playerController;
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _data = _dataBase;
            _signalBus.Subscribe<PlayerCrouchedSignal>(PlayerCrouched);
        }

        private async UniTaskVoid Start()
        {
            _lookAt = GetComponent<RabbitLookAt>();
            _animatorModule = new RabbitAnimatorModule(_animator);
            
            _idleState = new IdleState(_animatorModule, CanSeePlayer,ShouldFlee,0.3f, 1f);
            _walkState = new WalkState(_agent, _animatorModule, CanSeePlayer, 5f);
            _fleeState = new FleeState(_agent, _gameMath, _animatorModule, 12f);
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
            return _gameMath.DistanceToPlayer(transform) < _data.DistanceToFlee;
        }

        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }
        
        bool CanSeePlayer()
        {
            if (_playerController == null) return false;

            Vector3 eyePos = transform.position + Vector3.up * _data.EyeHeight;
            Vector3 playerPos = _playerController.transform.position + Vector3.up * 1f; // грудь игрока
            Vector3 direction = playerPos - eyePos;

            float sqrDist = direction.sqrMagnitude;
            if (sqrDist > _data.ViewDistance * _data.ViewDistance) return false;

            direction.Normalize();

            float halfViewCos = Mathf.Cos(_data.ViewAngle * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(transform.forward, direction) < halfViewCos) return false;

            if (Physics.Linecast(eyePos, playerPos, _obstacleLayer))
                return false;

            return true;
        }

        private void PlayerCrouched(PlayerCrouchedSignal signal)
        {
            if (signal.IsCrouched)
                _data = _dataCrouched;
            else
                _data = _dataBase;
        }
    }

    [Serializable]
    public struct RabbitData
    {
        public float DistanceToFlee;
        public float ViewAngle;
        public float EyeHeight;
        public float ViewDistance;
    }
}
