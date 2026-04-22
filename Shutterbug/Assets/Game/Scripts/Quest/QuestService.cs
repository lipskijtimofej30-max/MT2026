using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestService
    {
        private List<PhotoQuest> _quests = new List<PhotoQuest>();
        private QuestDatabase _questDatabase;
        private PhotoQuest _activeQuest;
        public IReadOnlyList<PhotoQuest> Quests => _quests;
        public PhotoQuest CurrentQuest => _activeQuest;

        [Inject]
        private void Construct(QuestDatabase questDatabase)
        {
            _questDatabase = questDatabase;
        }
        
        private void Start()
        {
            _activeQuest.IsActive = true;
            _quests = _questDatabase.Databased;
        }
        
        public void AcceptQuest(PhotoQuest quest)
        {
            _activeQuest = quest;
            _activeQuest.IsActive = true;
            Debug.LogWarning($"Текущий квест {_activeQuest.name}");
            _quests.Remove(quest);
        }
        
        public void CompleteActiveQuest()
        {
            Debug.LogWarning($"Квест {_activeQuest.name} выполнен!!");
            _activeQuest.IsActive = false;
            _activeQuest = null;
        }
    }
}
