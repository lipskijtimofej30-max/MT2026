using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

    namespace Game.Scripts.Animal.Wolf
    {
        public class WolfBrain : BaseAnimalBrain
        {
            [SerializeField] private NavMeshAgent _agent;
            [SerializeField] private Animator _animator;
            [SerializeField] private WolfConfig _config;
            [SerializeField] private AnimalType _animalType = AnimalType.Wolf;
            [SerializeField] private LayerMask _obstacleLayer;
            [SerializeField] private float _eyeHeight = 0.5f;

            private PlayerController _playerController;
            private SignalBus _signalBus;
            private AnimalDataRegistry _dataRegistry;
            private BaitRegistry _baitRegistry;
            private GameMath _gameMath;
            
            private WolfAnimatorModule _animatorModule;
            private IDataModule _dataModule;
            private IdleState _idleState;
            private WalkState _walkState;
            private AlertState _alertState;
            private AttackState _attackState;
            private EatingState _eatingState;
            
            [Inject]
            private void Construct(GameMath gameMath, PlayerController playerController, 
               AnimalDataRegistry dataRegistry, BaitRegistry baitRegistry, SignalBus signalBus)
            {
                _gameMath = gameMath;
                _playerController = playerController;
                _dataRegistry = dataRegistry;
                _baitRegistry = baitRegistry;
                _signalBus = signalBus;
            }

            private async UniTaskVoid Start()
            {
                _animatorModule = new WolfAnimatorModule(_animator);
                _dataModule = new BaseDataModule(_animalType,_config, _dataRegistry);

                _eatingState = new EatingState(_agent, _animatorModule, _baitRegistry, CanSeePlayer);
                _idleState = new IdleState(_animatorModule, _agent,CanSeePlayer, ShouldFlee,
                    _config, _baitRegistry, _eatingState);
                _walkState = new WalkState(_agent, _animatorModule, _baitRegistry, CanSeePlayer, _config, _eatingState);
                _alertState = new AlertState(_animatorModule, null, ShouldFlee, _config);
                _attackState = new AttackState(_agent, _animatorModule, _playerController, 
                    _config, ShouldFlee, _signalBus);

                var map = new Dictionary<StateAction, IState>
                {
                    {StateAction.GoToWalk, _walkState},
                    { StateAction.GoToIdle, _idleState},
                    { StateAction.GoToAlert,  _alertState},
                    { StateAction.GoToSpecialState, _attackState},
                    { StateAction.GoToBaitState, _eatingState}
                };
                _stateMachine = new StateMachine(map);
                AnimalType = _animalType;
                await _stateMachine.Start(_idleState).AttachExternalCancellation(destroyCancellationToken);
            }

            private void OnDestroy()
            {
                _stateMachine?.Dispose();
            }

            private bool ShouldFlee()
            { 
                return _gameMath.DistanceToPlayer(transform) < _dataModule.GetDistanceForSpecialState();
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
