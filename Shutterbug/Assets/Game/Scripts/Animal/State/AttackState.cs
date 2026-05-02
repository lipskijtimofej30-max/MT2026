using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts
{
    public class AttackState : IState
    {
        private readonly NavMeshAgent _agent;
        private readonly GameMath _gameMath;
        private readonly PlayerController _playerController;
        private readonly IAnimatorModule _animatorModule;
        private readonly WolfConfig _wolfConfig;
        private readonly Func<bool> _conditionMet;
        public AnimalState StateType => AnimalState.Attack;


        public AttackState(NavMeshAgent agent, IAnimatorModule animatorModule, PlayerController playerController,
            WolfConfig wolfConfig, Func<bool> conditionMet, GameMath gameMath)
        {
            _agent = agent;
            _animatorModule = animatorModule;
            _wolfConfig = wolfConfig;
            _gameMath = gameMath;
            _playerController = playerController;
            _conditionMet = conditionMet;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            _agent.speed = 4.5f;
            _agent.angularSpeed = 400f;
            _agent.acceleration = 10f;
            _animatorModule.StartAnimationSpecialState();
            while (!ct.IsCancellationRequested)
            {
                Vector3 playerPos = _playerController.transform.position;
                _agent.SetDestination(playerPos);

                if (!_conditionMet())
                {
                    Debug.Log("[AttackState] Игрок скрылся или слишком далеко. Прекращаем погоню.");
                    return StateAction.GoToWalk;
                }

                if (Vector3.Distance(_agent.transform.position, playerPos) < _wolfConfig.DistanceToHit)
                {
                    Debug.Log("[AttackState] КУСЬ!");
                }

                await UniTask.Yield(ct);
            }
            return StateAction.Stay;
        }

        public float GetStateMultiplier() => 2.3f;
    }
}