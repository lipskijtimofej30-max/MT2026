using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
                _currentAnimationCoroutine = StartCoroutine(ShowAnimation());
        }

        private void Close()
        {
            _isOpen = false;

            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            if(gameObject.activeInHierarchy)
                _currentAnimationCoroutine = StartCoroutine(HideAnimation());
        }

        private void RefreshAlbum()
        {
            foreach (Transform child in _content) Destroy(child.gameObject);

            if (_photoController.CurrentPhotoRecord != null)
            {
                var card =  Instantiate(_cardPrefab, _content);
                card.Init(_photoController.CurrentPhotoRecord,_photoController.SetCurrentPhoto, true);
            }

            foreach (var photo in _photoRegistry.Photos)
            {
                if(photo == null) continue;
                var card = Instantiate(_cardPrefab, _content);
                card.Init(photo,_photoController.SetCurrentPhoto, false);
            }
        }
        
        private IEnumerator ShowAnimation()
        {
            _windowRoot.localScale = Vector3.zero;
            _canvasGroup.alpha = 0f;

            var sequence = DOTween.Sequence();
            sequence.Join(_windowRoot.DOScale(1f, _showDuration).SetEase(_showEase));
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration).SetEase(_showEase));

            yield return sequence.WaitForCompletion();
        }
        
        private IEnumerator HideAnimation()
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_windowRoot.DOScale(0f, _hideDuration).SetEase(_hideEase));
            sequence.Join(_canvasGroup.DOFade(0f, _hideDuration).SetEase(_hideEase));

            yield return sequence.WaitForCompletion();
        }
    }
}