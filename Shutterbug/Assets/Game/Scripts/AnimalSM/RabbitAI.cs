using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    public class RabbitAI : BaseAnimalAI, IPhotoTarget
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _distanceToFlee = 15f;
        private GameMath _gameMath;
        
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
            _idleState = new IdleState(_gameMath,_agent, _animator, _distanceToFlee,0.3f, 1f);
            _walkState = new WalkState(_agent, _gameMath, _animator, _distanceToFlee, 5f);
            _fleeState = new FleeState(_agent, _gameMath, _animator, _distanceToFlee);

            Dictionary<StateAction, IState> map = new Dictionary<StateAction, IState>
            {
                { StateAction.GoToIdle, _idleState },
                { StateAction.GoToWalk, _walkState },
                { StateAction.GoToFlee, _fleeState },
            };
            
            _stateMachine = new StateMachine(map);
            AnimalType = AnimalType.Rabbit;
            CurrentState = _stateMachine.CurrentAnimalState;
            await StateMachine.Start(_idleState);
        }
    }
}
