using System.Collections;
using DG.Tweening;
using Game.Scripts.Core;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Quest
{
    public class QuestJournalUI : TabletWindow
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _windowRoot;
        [SerializeField] private QuestCardUI _cardPrefab;
        [SerializeField] private Transform _content;

        [Header("Components")]
        [SerializeField] private CameraCaptureView _cameraCaptureView;
        [SerializeField] private QuestDetailsPanel _detailsPanel;
        
        [Header("Animation Settings")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.4f;
        [SerializeField] private float _hideDuration = 0.3f;
        [SerializeField] private float _showMoveDistance = 100f; 
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.InBack; 

        private QuestService _service;
        private IAnimalInPhotoProvider _photoProvider;
        private PlayerController _playerController;
        
        private Vector2 _originalAnchoredPosition;
        private Coroutine _currentAnimationCoroutine;
        private bool _isOpen;

        [Inject]
        private void Construct(QuestService service, IAnimalInPhotoProvider photoProvider, PlayerController playerController)
        {
            _service = service;
            _photoProvider = photoProvider;
            _playerController = playerController;
            _detailsPanel.Construct(service, photoProvider, this);
        }

        private void Start()
        {
            _originalAnchoredPosition = _windowRoot.anchoredPosition;
            Close();
        }
        
        public override void ToggleWindow()
        {
            _isOpen = !_isOpen;
            if (_isOpen) Open(); else Close();
        }

        public override void SetVisible(bool visible)
        {
            _isOpen = visible;
            gameObject.SetActive(visible);

            if (_currentAnimationCoroutine != null)
            {
                StopCoroutine(_currentAnimationCoroutine);
                _currentAnimationCoroutine = null;
            }
            DOTween.Kill(_windowRoot);

            if (visible)
            {
                _windowRoot.anchoredPosition = new Vector2(_originalAnchoredPosition.x, 0);
                _windowRoot.localScale = Vector3.one;
                _canvasGroup.alpha = 1f;
                RefreshJournal();
            }
            else
            {
                _windowRoot.localScale = Vector3.zero;
                _canvasGroup.alpha = 0f;
            }
        }

        public void RefreshJournal()
        {
            foreach (Transform child in _content) Destroy(child.gameObject);

            if (_service.CurrentQuest != null)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Setup(_service.CurrentQuest, _detailsPanel.ShowActive, true);
            }

            foreach (var quest in _service.AvailableInJournal)
            {
                if(quest == null) continue;
                var card = Instantiate(_cardPrefab, _content);
                card.Setup(quest, _detailsPanel.ShowAvailable, false);
            }
        }

        private void Open()
        {
            _isOpen = true;

            _canvasGroup.alpha = 0f;
            _windowRoot.anchoredPosition = _originalAnchoredPosition - Vector2.up * _showMoveDistance;
            _windowRoot.localScale = Vector3.zero;
            _windowRoot.gameObject.SetActive(true);

            RefreshJournal();
            
            if (_service.CurrentQuest != null)
            {
                _detailsPanel.ShowActive(_service.CurrentQuest);
            } 
            else
            {
                // Если активного квеста нет, можно скрыть панель или показать заглушку
                _detailsPanel.gameObject.SetActive(false);
            }
            
            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            _currentAnimationCoroutine = StartCoroutine(ShowAnimation());
        }

        private void Close()
        {
            _isOpen = false;

            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            _currentAnimationCoroutine = StartCoroutine(HideAnimation());
        }

        public void ToggleWindow(bool active)
        {
            gameObject.SetActive(active);
        }

        private IEnumerator ShowAnimation()
        {
            _isOpen = true;

            _windowRoot.anchoredPosition = _originalAnchoredPosition - Vector2.up * _showMoveDistance;
            _windowRoot.localScale = Vector3.zero;
            _canvasGroup.alpha = 1f;

            var sequence = DOTween.Sequence();
            sequence.Join(_windowRoot.DOAnchorPos(_originalAnchoredPosition, _showDuration).SetEase(_showEase));
            sequence.Join(_windowRoot.DOScale(1f, _showDuration).SetEase(_showEase));
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration).SetEase(_showEase));

            yield return sequence.WaitForCompletion();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _cameraCaptureView.ToggleUI(false);
            _playerController.ToggleController(false);
        }

        private IEnumerator HideAnimation()
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_windowRoot
                .DOAnchorPos(_originalAnchoredPosition - Vector2.up * _showMoveDistance, _hideDuration)
                .SetEase(_hideEase));
            sequence.Join(_windowRoot.DOScale(0f, _hideDuration).SetEase(_hideEase));
            sequence.Join(_canvasGroup.DOFade(0f, _hideDuration).SetEase(_hideEase));

            yield return sequence.WaitForCompletion();
            
            _detailsPanel.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cameraCaptureView.ToggleUI(true);
            _playerController.ToggleController(true);
        }
    }
}
