using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts
{
    public class BunnyAI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        private StateMachine _stateMachine;
        private GameMath _gameMath;
        
        private IdleState _idleState;
        private WalkState _walkState;
        private FleeState _fleeState;

        [Inject]
        private void Construct(GameMath gameMath)
        {
            _gameMath = gameMath;
        }

        private async UniTask Start()
        {
            _idleState = new IdleState(0.3f, 1f);
            _walkState = new WalkState(_agent, _gameMath, 7f, 5f);
            _fleeState = new FleeState(_agent, _gameMath, 7f);

            Dictionary<StateAction, IState> map = new Dictionary<StateAction, IState>
            {
                { StateAction.GoToIdle, _idleState },
                { StateAction.GoToWalk, _walkState },
                { StateAction.GoToFlee, _fleeState },
            };
            
            _stateMachine = new StateMachine(map);
            await _stateMachine.Start(_idleState);
        }
    }
}