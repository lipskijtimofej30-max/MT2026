using Game.Scripts.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Quest
{
    public class QuestDetailsPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _buttonText;

        private PhotoQuest _currentQuest;
        private QuestService _service;
        private IAnimalInPhotoProvider _photoProvider;
        private QuestJournalUI _parentJournal;

        public void Construct(QuestService service, IAnimalInPhotoProvider photoProvider, QuestJournalUI journal)
        {
            _service = service;
            _photoProvider = photoProvider;
            _parentJournal = journal;
        }

        // Режим просмотра (квест еще не взят)
        public void ShowAvailable(PhotoQuest quest)
        {
            SetBaseInfo(quest);
            _buttonText.text = "Взять квест";
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(OnAcceptClicked);
            gameObject.SetActive(true);
        }

        // Режим активного квеста (уже в работе)
        public void ShowActive(PhotoQuest quest)
        {
            SetBaseInfo(quest);
            _buttonText.text = "Проверить фото";
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(OnVerifyClicked);
            gameObject.SetActive(true);
        }

        private void SetBaseInfo(PhotoQuest quest)
        {
            _currentQuest = quest;
            _title.text = quest.Description.ShortTitle;
            _description.text = quest.Description.FullDescription; // Художественное описание
        }

        private void OnAcceptClicked()
        {
            _service.AcceptQuest(_currentQuest);
            _parentJournal.RefreshJournal(); // Обновляем список слева
            ShowActive(_currentQuest);      // Переключаем эту же панель в режим проверки
        }

        private void OnVerifyClicked()
        {
            var data = _photoProvider.LastPhotoData;
            if (data == null) return;
            
            if (data != null && _currentQuest.IsCorrectTarget(data))
            {
                _service.CompleteActiveQuest();
                _parentJournal.RefreshJournal();
                gameObject.SetActive(false); // Закрываем детали после успеха
                Debug.Log("Квест сдан!");
            }
            else
            {
                Debug.LogWarning("Фото не подходит!");
                // Тут можно добавить анимацию тряски кнопки
            }
        }
    }
}