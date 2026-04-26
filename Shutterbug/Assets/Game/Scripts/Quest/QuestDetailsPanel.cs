using Game.Scripts.CameraPhoto.PhotoAlbum;
using Game.Scripts.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        private PhotoController _photoController;
        private QuestJournalUI _parentJournal;

        [Inject]
        private void Construct(PhotoController photoController)
        {
            _photoController = photoController;
        }
        
        public void Construct(QuestService service, QuestJournalUI journal)
        {
            _service = service;
            _parentJournal = journal;
        }

        public void ShowActive(PhotoQuest quest)
        {
            SetBaseInfo(quest);
            _buttonText.text = "Проверить фото";

            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(TryCompleteQuest);

            gameObject.SetActive(true);
        }
        
        public void ActivateQuest(PhotoQuest quest)
        {
            _service.AcceptQuest(quest);
            _parentJournal.RefreshJournal();    
            ShowActive(quest);
        }

        public void ShowActiveQuest(PhotoQuest quest)
        {
            ShowActive(quest);
        }
        
        
        private void SetBaseInfo(PhotoQuest quest)
        {
            _currentQuest = quest;
            _title.text = quest.Description.ShortTitle;
            _description.text = quest.Description.FullDescription;
        }

        private void TryCompleteQuest()
        {
            PhotoRecord currentPhoto = _photoController.CurrentPhotoRecord;

            if (currentPhoto == null)
            {
                Debug.LogWarning("Нет выбранного фото для проверки квеста!");
            }

            var capturedData = new CapturedPhotoData(currentPhoto.animalType, currentPhoto.animalStateType);

            if (_currentQuest.IsCorrectTarget(capturedData))
            {
                _service.CompleteActiveQuest();
                _parentJournal.RefreshJournal();
                gameObject.SetActive(false);
                Debug.LogWarning("Квест выполнен!");
            }
            else
            {
                Debug.LogWarning("Фото не соответствует условиям квеста!");
            }
        }
    }
}
