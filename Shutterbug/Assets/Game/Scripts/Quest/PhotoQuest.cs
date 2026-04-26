using System;
using Game.Scripts.Service;
using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Photo Quest", menuName = "Game/Quests/Photo Quest", order = 0)]
    public class PhotoQuest : ScriptableObject
    {
        [field: SerializeField] public AnimalType AnimalType { get; set; }
        [field: SerializeField] public AnimalState RequiredState { get; set; }
        [field: SerializeField] public Description Description { get; set; }
        
        public bool IsCorrectTarget(CapturedPhotoData data)
        {
            bool correct = data.AnimalType == AnimalType && data.AnimalState == RequiredState;
            Debug.LogWarning($"[PhotoQuest {this.name}] Animal type {data.AnimalType}, with state type {data.AnimalState}, need to {AnimalType} and {RequiredState} is correct: {correct}");
            return correct;
        }
    }

    [Serializable]
    public struct Description
    {
        public string ShortTitle;
        [TextArea]public string FullDescription;
    }
}