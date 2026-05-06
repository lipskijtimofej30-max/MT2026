using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using Game.Signals;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    public class AttackState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly SignalBus _signalBus;
        private readonly PlayerController _playerController;
        private readonly WolfAnimatorModule _animatorModule;
        private readonly WolfConfig _wolfConfig;
        private readonly Func<bool> _conditionMet;

        public AnimalState StateType => AnimalState.Attack;


        public AttackState(NavMeshAgent agent, WolfAnimatorModule animatorModule, PlayerController playerController,
            WolfConfig wolfConfig, Func<bool> conditionMet, SignalBus signalBus)
        {
            _agent = agent;
            _animatorModule = animatorModule;
            _wolfConfig = wolfConfig;
            _signalBus = signalBus;
            _playerController = playerController;
            _conditionMet = conditionMet;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            _agent.speed = 5f;
            _agent.angularSpeed = 400f;
            _agent.acceleration = 7.5f;
            _animatorModule.StartAnimationSpecialState();
            while (!ct.IsCancellationRequested)
            {
                Vector3 playerPos = _playerController.transform.position;
                _agent.SetDestination(playerPos);

                if (Vector3.Distance(_agent.transform.position, playerPos) < _wolfConfig.DistanceToHit)
                {
                    _animatorModule.StartAnimationAttack();
                    _signalBus.Fire(new AttackPlayerSignal(_playerController));
                }
                                
                if (!_conditionMet())
                {
                    Debug.Log("[AttackState] Игрок скрылся или слишком далеко. Прекращаем погоню.");
                    return StateAction.GoToWalk;
                }

                await UniTask.Yield(ct);
            }
            return StateAction.Stay;
        }

        public float GetStateMultiplier() => 2.3f;
    }
}