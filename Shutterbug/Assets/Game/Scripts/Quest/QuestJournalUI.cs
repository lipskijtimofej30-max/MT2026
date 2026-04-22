using UnityEngine;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestJournalUI : MonoBehaviour
    {
        [SerializeField] private QuestCardUI _cardPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private QuestDetailsPanel _detailsPanel;
        private QuestService _questService;

        [Inject]
        private void Construct(QuestService questService)
        {
            _questService = questService;
        }

        public void RefreshJournal()
        {
            foreach (Transform child in _content) Destroy(child.gameObject);

            if (_questService.CurrentQuest != null)
            {
                
            }
        }

        public void CreateCard(PhotoQuest quest, bool isAlreadyActive)
        {
            var card = Instantiate(_cardPrefab, _content);
            //card.Setup(quest, ());
        }
    }
}