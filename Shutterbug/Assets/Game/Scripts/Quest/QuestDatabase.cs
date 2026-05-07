using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Quest
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Game/Quests/Quest Database", order = 0)]
    public class QuestDatabase : Database<PhotoQuest>
    {
        [field: SerializeField] public List<SpecialQuest> SpecialQuests { get; private set; }
    }
    
    [Serializable]
    public struct SpecialQuest
    {
        public PhotoQuest Quest;
        public int ValueForQuest;

        public SpecialQuest(PhotoQuest quest, int valueForQuest)
        {
            Quest = quest;
            ValueForQuest = valueForQuest;
        }
    }
}