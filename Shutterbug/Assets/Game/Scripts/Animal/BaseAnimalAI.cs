using UnityEngine;

namespace Game.Scripts
{
    public class BaseAnimalAI : MonoBehaviour , IPhotoTarget
    {
        protected StateMachine _stateMachine;
        public StateMachine StateMachine => _stateMachine;
        public AnimalType AnimalType { get; set; }
        public AnimalState CurrentState => _stateMachine.CurrentAnimalState;
    }
}
