using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    public class RabbitAI : BaseAnimalAI
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimalType _animalType = AnimalType.Rabbit;
        [SerializeField] private float _distanceToFlee = 15f;
        private GameMath _gameMath;
        
        private RabbitAnimatorModule _animatorModule;
        private IdleState _idleState;
        private WalkState _walkState;
        private FleeState _fleeState;
        
        [Inject]
        private void Construct(GameMath gameMath)
        {
            _gameMath = gameMath;
        }
        
        private async UniTaskVoid Start()
        {
            Debug.Log($"Distance to flee: {_distanceToFlee}");
            _animatorModule = new RabbitAnimatorModule(_animator);
            
            _idleState = new IdleState(_animatorModule, SpecialTransitionMethod,0.3f, 1f);
            _walkState = new WalkState(_agent, _gameMath, _animatorModule, SpecialTransitionMethod, 5f);
            _fleeState = new FleeState(_agent, _gameMath, _animatorModule, SpecialTransitionMethod);

            Dictionary<StateAction, IState> map = new Dictionary<StateAction, IState>
            {
                { StateAction.GoToIdle, _idleState },
                { StateAction.GoToWalk, _walkState },
                { StateAction.GoToSpecialState, _fleeState },
            };
            
            _stateMachine = new StateMachine(map);
            AnimalType = _animalType;
            await StateMachine.Start(_idleState).AttachExternalCancellation(destroyCancellationToken);
        }

        private bool SpecialTransitionMethod()
        {
            if (_gameMath.DistanceToPlayer(_agent.transform) < _distanceToFlee)
                return true;
            else
                return false;
        }

        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }
    }
}
