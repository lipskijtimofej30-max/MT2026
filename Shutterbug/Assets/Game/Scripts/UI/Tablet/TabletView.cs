using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
    public class TabletView : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _windowRoot;
        [SerializeField] private List<TabletWindow> _windows = new();
        
        [Header("Animation Settings")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.4f;
        [SerializeField] private float _hideDuration = 0.3f;
        [SerializeField] private float _showMoveDistance = 100f; 
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.InBack; 
        
        private Vector2 _originalAnchoredPosition;
        private Coroutine _currentAnimationCoroutine;
        private bool _isOpen;

        private void Start()
        {
            _originalAnchoredPosition = _windowRoot.anchoredPosition;
            Close();
        }

        public void OnEnterState()
        {
            Open();
        }

        public void OnExitState()
        {
            Close();
        }
 
        private void Open()
        {
            _isOpen = true;

            _windowRoot.anchoredPosition = _originalAnchoredPosition - Vector2.up * _showMoveDistance;
            _windowRoot.localScale = Vector3.zero;
            _windowRoot.gameObject.SetActive(true);

            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            _currentAnimationCoroutine = StartCoroutine(ShowAnimation());
        }
        
        private void Close()
        {
            _isOpen = false;

            foreach (TabletWindow window in _windows)
            {
                try
                {
                    window.SetVisible(false);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error hiding window {window.name}:" + e.Message);
                }
            }
            
            if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
            DOTween.Kill(_windowRoot);
            _currentAnimationCoroutine = StartCoroutine(HideAnimation());
        }

        private IEnumerator ShowAnimation()
        {
            _isOpen = true;
            
            _windowRoot.anchoredPosition = _originalAnchoredPosition - Vector2.up * _showMoveDistance;
            _windowRoot.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();
            sequence.Join(_windowRoot.DOAnchorPos(_originalAnchoredPosition, _showDuration).SetEase(_showEase));
            sequence.Join(_windowRoot.DOScale(1f, _showDuration).SetEase(_showEase));
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration).SetEase(_showEase));

            yield return sequence.WaitForCompletion();
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
        }
    }
}
