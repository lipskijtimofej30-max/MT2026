using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Service
{
    public class PhotoProviderView : MonoBehaviour
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private CanvasGroup _canvasGroup; // Для плавного появления/исчезновения

        [Header("Animation Settings")] [SerializeField]
        private float _flyDuration = 0.5f;

        [SerializeField] private float _displayTime = 2f;
        [SerializeField] private float _fadeOutDuration = 0.4f;

        private IPhotoProvider _photoProvider;
        private Vector2 _targetPosition;
        private Coroutine _currentAnimation;

        [Inject]
        private void Construct(IPhotoProvider photoProvider)
        {
            _photoProvider = photoProvider;
        }

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _targetPosition = _image.rectTransform.anchoredPosition;
            _image.gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;

            _photoProvider.OnPhotoLoaded += RenderImage;
        }

        private void OnDestroy()
        {
            _photoProvider.OnPhotoLoaded -= RenderImage;
            DOTween.Kill(_image.rectTransform);
            DOTween.Kill(_canvasGroup);
        }

        private void RenderImage(Texture2D texture)
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);

            _currentAnimation = StartCoroutine(ShowFlashReveal(texture));
        }

        private IEnumerator ShowFlashReveal(Texture2D texture)
        {
            _image.texture = texture;
            _image.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;

            // Создаём временную «вспышку» (белый Image поверх всего)
            GameObject flash = new GameObject("Flash", typeof(RectTransform), typeof(Image));
            flash.transform.SetParent(transform, false);
            flash.GetComponent<Image>().color = Color.white;
            RectTransform flashRt = flash.GetComponent<RectTransform>();
            flashRt.anchorMin = Vector2.zero;
            flashRt.anchorMax = Vector2.one;
            flashRt.sizeDelta = Vector2.zero;
            flashRt.anchoredPosition = Vector2.zero;

            CanvasGroup flashCg = flash.AddComponent<CanvasGroup>();
            flashCg.alpha = 1f;

            // Фото слегка уменьшено и размыто (можно добавить, если есть шейдер)
            _image.rectTransform.localScale = Vector3.one * 0.95f;

            Sequence reveal = DOTween.Sequence();
            // Вспышка затухает, открывая фото
            reveal.Append(flashCg.DOFade(0f, 0.4f).SetEase(Ease.InSine));
            // Параллельно фото плавно появляется и слегка увеличивается
            reveal.Join(_canvasGroup.DOFade(1f, 0.6f).SetEase(Ease.OutSine));
            reveal.Join(_image.rectTransform.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

            yield return reveal.WaitForCompletion();

            Destroy(flash);

            yield return new WaitForSeconds(_displayTime);

            yield return _canvasGroup.DOFade(0f, _fadeOutDuration).WaitForCompletion();

            _image.gameObject.SetActive(false);
            _currentAnimation = null;
        }
    }
}