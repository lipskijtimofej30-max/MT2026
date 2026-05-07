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
        private List<SpecialQuest> _pendingSpecialQuests = new();
        
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
            _pendingSpecialQuests = new List<SpecialQuest>(_questDatabase.SpecialQuests);
            ReplenishPool();
            RefreshAvailableQuests();
        }

        private void ReplenishPool()
        {
            // Фильтруем, чтобы не добавить в пул то, что уже в журнале или активно
            var newQuests = _questDatabase.Databased
                .Where(q => !_availableInJournal.Contains(q) && q != _activeQuest)
                .OrderBy(x => Random.value)
                .ToList();

            _pool.Clear(); // На всякий случай чистим старый хвост
            foreach (var q in newQuests) _pool.Enqueue(q);
        }

        public void RefreshAvailableQuests()
        {
            int targetCount = Random.Range(1, 4); 

            while (_availableInJournal.Count < targetCount)
            {
                if (_pool.Count == 0) ReplenishPool();

                // 1. Ищем, нет ли спец-квеста для текущего прогресса
                PhotoQuest specialToIssue = TryGetSpecialQuest();

                PhotoQuest finalQuest;

                if (specialToIssue != null)
                {
                    finalQuest = specialToIssue;
                    Debug.Log($"<color=yellow>СЮЖЕТНЫЙ КВЕСТ:</color> {finalQuest.name} выдан на числе {_totalQuestsIssued}");
                }
                else
                {
                    if (_pool.Count == 0) break;
                    finalQuest = _pool.Dequeue();
                }

                _availableInJournal.Add(finalQuest);
            }
        }
        
        private PhotoQuest TryGetSpecialQuest()
        {
            // Ищем первый спец-квест, чей порог мы достигли или перешагнули
            var special = _pendingSpecialQuests
                .FirstOrDefault(s => _totalQuestsIssued >= s.ValueForQuest);

            if (special != null)
            {
                // Удаляем из списка ожидания, чтобы не выдавать его вечно
                _pendingSpecialQuests.Remove(special);
                return special.Quest;
            }

            return null;
        }

        public void AcceptQuest(PhotoQuest quest)
        {
            if (_activeQuest != null)
                _availableInJournal.Add(_activeQuest);

            _availableInJournal.Remove(quest);
            _activeQuest = quest;
        }

        public void CompleteActiveQuest()
        {
            if (_activeQuest == null) return;

            var score = _provider.CurrentPhotoRecord.photoScore.TotalScore;
            var amount = _rewardCalculator.Validate(_activeQuest, _activeQuest.RangeReward, score);

            _currencyService.AddCurrency(amount);

            Debug.LogWarning($"Квест {_activeQuest.name} выполнен! Награда: {amount}");

            _activeQuest = null;
            _totalQuestsIssued++;

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
