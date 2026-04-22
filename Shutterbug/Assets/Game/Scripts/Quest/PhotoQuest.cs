using System;
using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Photo Quest", menuName = "Game/Quests/Photo Quest", order = 0)]
    public class PhotoQuest : ScriptableObject
    {
        [field: SerializeField] public AnimalType AnimalType { get; set; }
        [field: SerializeField] public AnimalState RequiredState { get; set; }
        [field: SerializeField] public Description Description { get; set; }
        
        public bool IsCorrectTarget(BaseAnimalAI animal)
        {
            bool correct = animal.AnimalType == AnimalType && animal.CurrentState == RequiredState;
            return correct;
        }
    }

    [Serializable]
    public struct Description
    {
        public string ShortTitle;
        public string FullDescription;
    }
}