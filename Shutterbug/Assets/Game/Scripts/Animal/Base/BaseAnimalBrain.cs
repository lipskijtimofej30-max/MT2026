using UnityEngine;

namespace Game.Scripts
{
    public abstract class BaseAnimalBrain : MonoBehaviour
    {
        protected StateMachine _stateMachine;
        public StateMachine StateMachine => _stateMachine;
        public AnimalType AnimalType { get; set; }
        public AnimalState CurrentState => _stateMachine.CurrentAnimalState;
    }
}
