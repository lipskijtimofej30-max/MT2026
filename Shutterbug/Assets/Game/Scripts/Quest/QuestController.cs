using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Quest
{
    public class QuestController : MonoBehaviour
    {
        [SerializeField] private List<PhotoQuest> _quests = new List<PhotoQuest>();
        private int _currentQuestIndex = 0;
        
        public PhotoQuest CurrentQuest => _quests[_currentQuestIndex];
    }
}