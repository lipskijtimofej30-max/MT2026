using System;
using System.Collections.Generic;
using Game.Scripts.Service;
using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Photo Quest", menuName = "Game/Quests/Photo Quest", order = 0)]
    public class PhotoQuest : ScriptableObject
    {
        [field: SerializeField] public AnimalType AnimalType { get; set; }
        [field: SerializeField] public AnimalState RequiredState { get; set; }
        [field: SerializeField] public float BaseReward { get; set; }
        [field:SerializeField] public List<RewardPhotoValue> RangeReward { get; set; }
        [field: SerializeField] public Description Description { get; set; }


        public bool IsCorrectTarget(CapturedPhotoData data)
        {
            bool correct = data.AnimalType == AnimalType && data.AnimalState.StateType == RequiredState;
            Debug.LogWarning($"[PhotoQuest {this.name}] Animal type {data.AnimalType}, with state type {data.AnimalState}, need to {AnimalType} and {RequiredState} is correct: {correct},");
            return correct;
        }
    }
    
    #region Handler

        [Serializable]
        public struct Description
        {
            public string ShortTitle;
            [TextArea]public string FullDescription;
        }
    
    #endregion Handler
}
