using System.Collections;
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

        public IEnumerator FadeOut()
        {
            _canvasGroup.gameObject.SetActive(true);
            yield return _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.InSine).WaitForCompletion();
        }

        public IEnumerator FadeIn()
        {
            yield return _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.OutSine).WaitForCompletion();
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}