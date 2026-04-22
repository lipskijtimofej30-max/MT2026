using Game.Scripts.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestDetailsPanel : MonoBehaviour
    {
        [SerializeField] private QuestJournalUI _parent;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _buttonText;
        
        private PhotoQuest _selectedQuest;
        private IAnimalInPhotoProvider _provider;
        private QuestService _service;

        [Inject]
        private void Construct(IAnimalInPhotoProvider provider, QuestService service)
        {
            _service = service;
            _provider = provider;
        }

        
        public void Show(PhotoQuest quest)
        {
            _selectedQuest = quest;
            _title.text = quest.name;
            _description.text = quest.Description.FullDescription;
            
            _buttonText.text = "Взять квест";
            _actionButton.onClick.RemoveAllListeners();
        }

        public void ShowActive(PhotoQuest quest)
        {
            Show(quest);
            _buttonText.text = "Проверить фото";
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(OnVerifyClicked);
        }

        private void OnVerifyClicked()
        {
            var lastTarget = _provider.TargetAnimal;
            
            if (_service.CurrentQuest != null && _service.CurrentQuest.IsCorrectTarget(lastTarget))
            {
                Debug.LogWarning($"Квест успешно выполнен");
                _service.CompleteActiveQuest();
            }
            else
            {
                Debug.LogWarning("Фото не подходит пол условие квеста или др.");
            }
        }

        private void OnAcceptClicked()
        {
            _service.AcceptQuest(_selectedQuest);
            _parent.RefreshJournal();
            ShowActive(_selectedQuest);
        }
    }
}