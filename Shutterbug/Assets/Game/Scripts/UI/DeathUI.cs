using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class DeathUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 1.0f;

        private void Start()
        {
            _canvasGroup.alpha = 0f;
        }

        public Tween FadeOut() 
        {
            _canvasGroup.alpha = 0f;
            return _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.InSine);
        }

        public Tween FadeIn() 
        {
            _canvasGroup.alpha = 1f;
            return _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.OutSine);
        }
    }
}