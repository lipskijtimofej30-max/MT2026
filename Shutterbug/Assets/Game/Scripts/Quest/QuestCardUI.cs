using Game.Scripts.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestCardUI : MonoBehaviour
    {
        [SerializeField] private PhotoQuest _activeQuest;
        [SerializeField] private Transform _container;
        [SerializeField] private Button _questCompleteButton;
        [SerializeField] private Button _questAcceptButton;

        private int _countOpen = 0;
        private IAnimalInPhotoProvider _animalInPhotoProvider;
        private QuestService _questService;
        private PlayerController _playerController;

        [Inject]
        private void Construct(IAnimalInPhotoProvider animalInPhotoProvider, QuestService questService, PlayerController playerController)
        {
            _animalInPhotoProvider = animalInPhotoProvider;
            _questService = questService;
            _playerController = playerController;
        }

        private void Start()
        {
            Hide();
            _questAcceptButton.onClick.AddListener(() => _questService.AcceptQuest(_activeQuest));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _countOpen++;
                if (_countOpen % 2 == 1)
                    Show();
                else
                    Hide();
            }
        }

        private void IsQuestComplete()
        {
            var animal = _animalInPhotoProvider.TargetAnimal;
            var quest = _questService.CurrentQuest;
            if (animal != null && quest !=null && quest.IsCorrectTarget(animal))
            {
                _questService.CompleteActiveQuest();
            }
        }
        
        private void Show()
        {
            _container.gameObject.SetActive(true);
            _playerController.ToggleController(false);
        }

        private void Hide()
        {
            _container.gameObject.SetActive(false);
            _playerController.ToggleController(true);
        }
    }
}