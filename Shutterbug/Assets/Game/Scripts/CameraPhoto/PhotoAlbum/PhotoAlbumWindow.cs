using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoAlbumWindow : TabletWindow
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _windowRoot;
        [SerializeField] private PhotoAlbumCardUI _cardPrefab;
        [SerializeField] private Transform _content;

        [Header("Animation Settings")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.4f;
        [SerializeField] private float _hideDuration = 0.3f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.InBack;
        
        private PhotoController _photoController;
        private PhotoRegistry _photoRegistry;
        
        private List<PhotoAlbumCardUI> _activeCards = new();
        private Coroutine _currentAnimationCoroutine;
        private bool _isOpen;

        [Inject]
        private void Construct(PhotoController photoController, PhotoRegistry photoRegistry)
        {
            _photoController = photoController;
            _photoRegistry = photoRegistry;
        }

        private void Start()
        {
            _photoController.OnPhotoAlbumChanged += RefreshAlbum;
        }

        private void OnDestroy()
        {
            _photoController.OnPhotoAlbumChanged -= RefreshAlbum;
        }

        public override void ToggleWindow()
        {
            _isOpen = !_isOpen;
            if (_isOpen) Open();
            else Close();
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
                _windowRoot.localScale = Vector3.one;
                _canvasGroup.alpha = 1f;
                Open();
                RefreshAlbum();
            }
            else
            {
                _windowRoot.localScale = Vector3.zero;
                _canvasGroup.alpha = 0f;
            }
        }

        private void Open()
        {
            _isOpen = true;

            _canvasGroup.alpha = 0f;
            _windowRoot.localScale = Vector3.zero;
            _windowRoot.gameObject.SetActive(true);
            
            RefreshAlbum();
            
            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            if(gameObject.activeInHierarchy)
                _currentAnimationCoroutine = StartCoroutine(ShowAnimation(_windowRoot, _canvasGroup, _showDuration, _showEase));
        }

        private void Close()
        {
            _isOpen = false;

            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            if(gameObject.activeInHierarchy)
                _currentAnimationCoroutine = StartCoroutine(HideAnimation(_windowRoot, _canvasGroup, _hideDuration, _hideEase));
        }

        private void RefreshAlbum()
        {
            foreach (var card in _activeCards)
                Destroy(card.gameObject);
            _activeCards.Clear();

            if (_photoController.CurrentPhotoRecord != null)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Init(_photoController.CurrentPhotoRecord, _photoController.SetCurrentPhoto, true);
                _activeCards.Add(card);
            }

            foreach (var photo in _photoRegistry.Photos)
            {
                if (photo == null || photo == _photoController.CurrentPhotoRecord) continue; 
                var card = Instantiate(_cardPrefab, _content);
                bool isActive = _photoController.CurrentPhotoRecord == photo;
                card.Init(photo, _photoController.SetCurrentPhoto, isActive);
                _activeCards.Add(card);
            }
        }
    }
}
