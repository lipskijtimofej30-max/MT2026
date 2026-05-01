using Game.Data;
using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "Photo Reward Config", menuName = "Game/Config/Photo Reward Config")]
    public class PhotoRewardConfig : ScriptableObject
    {
        [field: SerializeField] private ValueMinMax newSpeciesReward;
        [field: SerializeField] private ValueMinMax newBehaviorReward;
        
        public int NewSpeciesReward => (int)Random.Range(newSpeciesReward.Min, newSpeciesReward.Max);
        public int NewBehaviorReward => (int)Random.Range(newBehaviorReward.Min, newBehaviorReward.Max);
    }
}