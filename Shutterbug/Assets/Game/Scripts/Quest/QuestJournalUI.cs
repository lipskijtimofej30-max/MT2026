using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Quest
{
public class QuestJournalUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _windowRoot;
        [SerializeField] private QuestCardUI _cardPrefab;
        [SerializeField] private Transform _content;

        [Header("Components")]
        [SerializeField] private QuestDetailsPanel _detailsPanel;

        private QuestService _service;
        private IAnimalInPhotoProvider _photoProvider;
        private bool _isOpen;

        [Inject]
        private void Construct(QuestService service, IAnimalInPhotoProvider photoProvider)
        {
            _service = service;
            _photoProvider = photoProvider;
            _detailsPanel.Construct(service, photoProvider, this);
        }

        private void Start() => Close();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H)) ToggleJournal();
        }

        public void ToggleJournal()
        {
            _isOpen = !_isOpen;
            if (_isOpen) Open(); else Close();
        }

        public void RefreshJournal()
        {
            // Очистка
            foreach (Transform child in _content) Destroy(child.gameObject);

            // 1. Сначала спавним активный квест (если есть)
            if (_service.CurrentQuest != null)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Setup(_service.CurrentQuest, _detailsPanel.ShowActive, true);
            }

            // 2. Затем спавним доступные
            foreach (var quest in _service.AvailableInJournal)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Setup(quest, _detailsPanel.ShowAvailable, false);
            }
        }

        private void Open()
        {
            _windowRoot.SetActive(true);
            RefreshJournal();

            // Автоматически показываем активный квест в панели деталей
            if (_service.CurrentQuest != null)
            {
                _detailsPanel.ShowActive(_service.CurrentQuest);
            }
            else
            {
                // Если активного квеста нет, можно скрыть панель или показать заглушку
                _detailsPanel.gameObject.SetActive(false);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Тут можно вызвать _playerController.ToggleController(false)
        }

        private void Close()
        {
            _windowRoot.SetActive(false);
            _detailsPanel.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Тут можно вызвать _playerController.ToggleController(true)
        }
    }
}
