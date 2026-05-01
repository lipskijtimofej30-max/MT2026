using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Scripts.CameraPhoto.PhotoAlbum;
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
        private RewardCalculator _rewardCalculator;
        private IPhotoRecordProvider _provider;
        private ICurrencyService _currencyService;
        
        public IReadOnlyList<PhotoQuest> AvailableInJournal => _availableInJournal;
        public PhotoQuest CurrentQuest => _activeQuest;

        [Inject]
        private void Construct(QuestDatabase questDatabase, IPhotoRecordProvider provider, ICurrencyService currencyService)
        {
            _questDatabase = questDatabase;
            _provider = provider;
            _currencyService = currencyService;
        }
        public void Initialize()
        {
            var shuffled = _questDatabase.Databased.OrderBy(x => Random.value).ToList();
            foreach(var quest in shuffled) _pool.Enqueue(quest);
            _rewardCalculator = new RewardCalculator();
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
            var amount = _rewardCalculator.Validate(_activeQuest, _activeQuest.RangeReward, _provider.CurrentPhotoRecord.photoScore.TotalScore);
            _currencyService.AddCurrency(amount);
            Debug.LogWarning($"Квест {_activeQuest.name} выполнен!!");
            _activeQuest = null;
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

        public RewardPhotoValue(ValueMinMax range,  float multiplier)
        {
            Range = range;
            Multiplier = multiplier;
        }
    }

}
