using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestService : IInitializable
    {
        private QuestDatabase _questDatabase;
        private Queue<PhotoQuest> _pool = new();
        private List<PhotoQuest> _availableInJournal = new();
        private PhotoQuest _activeQuest;
        
        public IReadOnlyList<PhotoQuest> AvailableInJournal => _availableInJournal;
        public PhotoQuest CurrentQuest => _activeQuest;

        [Inject]
        private void Construct(QuestDatabase questDatabase)
        {
            _questDatabase = questDatabase;
        }
        public void Initialize()
        {
            var shuffled = _questDatabase.Databased.OrderBy(x => Random.value).ToList();
            foreach(var quest in shuffled) _pool.Enqueue(quest);

            RefreshAvailableQuests();
        }

        private void RefreshAvailableQuests()
        {
            while (_availableInJournal.Count < 3 && _pool.Count > 0)
            {
                _availableInJournal.Add(_pool.Dequeue());
            }
        }
        
        public void AcceptQuest(PhotoQuest quest)
        {
            if(_activeQuest != null)
                _availableInJournal.Add(_activeQuest);
            
            _availableInJournal.Remove(quest);
            _activeQuest = quest;
            
            Debug.LogWarning($"Текущий квест {_activeQuest.name}");
        }
        
        public void CompleteActiveQuest()
        {
            Debug.LogWarning($"Квест {_activeQuest.name} выполнен!!");
            _activeQuest = null;
        }
    }
}
