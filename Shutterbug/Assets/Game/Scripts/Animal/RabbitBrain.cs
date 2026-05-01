using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
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
        [SerializeField] private RabbitConfig _config;
        [SerializeField] private AnimalType _animalType = AnimalType.Rabbit;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _eyeHeight = 0.5f;

        private PlayerController _playerController;
        private AnimalDataRegistry _dataRegistry;
        private BaitRegistry _baitRegistry;
        private SignalBus _signalBus;
        private GameMath _gameMath;
        private RabbitLookAt _lookAt;
        
        private RabbitAnimatorModule _animatorModule;
        private IDataModule _dataModule;
        private IdleState _idleState;
        private WalkState _walkState;
        private FleeState _fleeState;
        private AlertState _alertState;
        private EatingState _eatingState;
        
        [Inject]
        private void Construct(GameMath gameMath, PlayerController playerController, 
           SignalBus signalBus, AnimalDataRegistry dataRegistry, BaitRegistry baitRegistry)
        {
            _gameMath = gameMath;
            _playerController = playerController;
            _dataRegistry = dataRegistry;
            _signalBus = signalBus;
            _baitRegistry = baitRegistry;
        }

        private async UniTaskVoid Start()
        {
            _lookAt = GetComponent<RabbitLookAt>();
            var handler = new DataHandler(_signalBus,  _dataRegistry);
            _animatorModule = new RabbitAnimatorModule(_animator);
            _dataModule = new BaseDataModule(_animalType, _config, _dataRegistry);
            
            _eatingState = new EatingState(_agent, _animatorModule, _baitRegistry, CanSeePlayer);
            _idleState = new IdleState(_animatorModule, _agent,CanSeePlayer,ShouldFlee,
                _config, _baitRegistry,_eatingState);
            _walkState = new WalkState(_agent, _animatorModule, _baitRegistry, CanSeePlayer, _config, _eatingState);
            _fleeState = new FleeState(_agent, _gameMath, _animatorModule, _config);
            _alertState = new AlertState(_animatorModule, _lookAt, ShouldFlee,  _config);

            Dictionary<StateAction, IState> map = new Dictionary<StateAction, IState>
            {
                { StateAction.GoToIdle, _idleState },
                { StateAction.GoToWalk, _walkState },
                { StateAction.GoToSpecialState, _fleeState },
                { StateAction.GoToAlert, _alertState },
                { StateAction.GoToBaitState, _eatingState}
            };
            
            _stateMachine = new StateMachine(map);
            AnimalType = _animalType;
            await StateMachine.Start(_idleState).AttachExternalCancellation(destroyCancellationToken);
        }

        private bool ShouldFlee()
        { 
            return _gameMath.DistanceToPlayer(transform) < _dataModule.GetDistanceForSpecialState();
        }

        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }
        
        bool CanSeePlayer()
        {
            if (_playerController == null) return false;

            Vector3 eyePos = transform.position + Vector3.up * _eyeHeight;
            Vector3 playerPos = _playerController.transform.position + Vector3.up * 1f; // грудь игрока
            Vector3 direction = playerPos - eyePos;

            float sqrDist = direction.sqrMagnitude;
            if (sqrDist > _dataModule.GetViewDistance() * _dataModule.GetViewDistance()) return false;

            direction.Normalize();

            float halfViewCos = Mathf.Cos(_dataModule.GetViewAngle() * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(transform.forward, direction) < halfViewCos) return false;

            if (Physics.Linecast(eyePos, playerPos, _obstacleLayer))
                return false;
            return true;
        }
    }
}
