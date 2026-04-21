using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Photo Quest", menuName = "Game/Quest/Photo Quest", order = 0)]
    public class PhotoQuest : ScriptableObject
    {
        [field: SerializeField] public AnimalType AnimalType { get; set; }
        [field: SerializeField] public AnimalState RequiredState { get; set; }

        public bool IsCorrectTarget(BaseAnimalAI animal)
        {
            return animal.AnimalType == AnimalType && animal.CurrentState == RequiredState;
        }
    }
}