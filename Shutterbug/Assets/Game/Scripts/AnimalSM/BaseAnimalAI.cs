using UnityEngine;

namespace Game.Scripts
{
    public class BaseAnimalAI : MonoBehaviour
    {
        protected StateMachine _stateMachine;
        public StateMachine StateMachine => _stateMachine;
    }
}
