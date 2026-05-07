using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Service;
using Game.Service.Currency;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Quest
{
    public class QuestService : IInitializable
    {
        private QuestDatabase _questDatabase;
        private Queue<PhotoQuest> _pool = new();
        private List<PhotoQuest> _availableInJournal = new();
        private PhotoQuest _activeQuest;

        private int _totalQuestsIssued = 0;
        private IPhotoRecordProvider _provider;
        private ICurrencyService _currencyService;
        private RewardCalculator _rewardCalculator;

        public IReadOnlyList<PhotoQuest> AvailableInJournal => _availableInJournal;
        public PhotoQuest CurrentQuest => _activeQuest;

        [Inject]
        private void Construct(QuestDatabase questDatabase, IPhotoRecordProvider provider,
            ICurrencyService currencyService)
        {
            _questDatabase = questDatabase;
            _provider = provider;
            _currencyService = currencyService;
        }

        public void Initialize()
        {
            _rewardCalculator = new RewardCalculator();
            ReplenishPool();
            RefreshAvailableQuests();
        }

        private void ReplenishPool()
        {
            Debug.Log("Пул пуст. Перемешиваем базу данных и наполняем заново...");

            var newQuests = _questDatabase.Databased
                .Where(q => !_availableInJournal.Contains(q) && q != _activeQuest)
                .OrderBy(x => Random.value) 
                .ToList();
            
            foreach (var quest in newQuests)
            {
                _pool.Enqueue(quest);
                Debug.LogWarning($"Количество квестов которое было выгруженно {_totalQuestsIssued}");
            }
        }

        public void RefreshAvailableQuests()
        {
            int targetCount = Random.Range(1, 5); 

            while (_availableInJournal.Count < targetCount)
            {
                if (_pool.Count == 0)
                {
                    ReplenishPool();
                    if (_pool.Count == 0) break;
                }

                PhotoQuest questToIssue = CheckForSpecialQuest();

                if (questToIssue == null)
                {
                    questToIssue = _pool.Dequeue();
                }

                _availableInJournal.Add(questToIssue);
                _totalQuestsIssued++;
                
                Debug.Log($"Выдан квест №{_totalQuestsIssued}: {questToIssue.name}");
            }
        }
        
        private PhotoQuest CheckForSpecialQuest()
        {
            var special = _questDatabase.SpecialQuests
                .FirstOrDefault(s => s.ValueForQuest == _totalQuestsIssued);

            return special.Quest;
        }

        public void AcceptQuest(PhotoQuest quest)
        {
            if (_activeQuest != null)
                _availableInJournal.Add(_activeQuest);

            _availableInJournal.Remove(quest);
            _activeQuest = quest;

            RefreshAvailableQuests();
        }

        public void CompleteActiveQuest()
        {
            if (_activeQuest == null) return;

            var score = _provider.CurrentPhotoRecord.photoScore.TotalScore;
            var amount = _rewardCalculator.Validate(_activeQuest, _activeQuest.RangeReward, score);

            _currencyService.AddCurrency(amount);

            Debug.LogWarning($"Квест {_activeQuest.name} выполнен! Награда: {amount}");

            _activeQuest = null;

            RefreshAvailableQuests();
        }
    }


    public class RewardCalculator
    {
        public int Validate(PhotoQuest target, List<RewardPhotoValue> values, int score)
        {
            foreach (var value in values)
            {
                if (score >= value.Range.Min && score <= value.Range.Max)
                    return (int)(target.BaseReward * value.Multiplier);
            }

            return (int)target.BaseReward;
        }
    }

    [Serializable]
    public struct RewardPhotoValue
    {
        public ValueMinMax Range;
        public float Multiplier;

        public RewardPhotoValue(ValueMinMax range, float multiplier)
        {
            Range = range;
            Multiplier = multiplier;
        }
    }
}
