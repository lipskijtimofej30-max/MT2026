using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Service
{
    public class PhotoProviderView : MonoBehaviour
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _scoreText;

        [Header("Animation Settings")] 
        [SerializeField] private float _flyDuration = 0.5f;
        [SerializeField] private float _displayTime = 3.5f;
        [SerializeField] private float _fadeOutDuration = 0.4f;

        private IPhotoProvider _photoProvider;
        private IAnimalInPhotoProvider _animalInPhotoProvider;
        private PlayerController _playerController;
        
        private Coroutine _currentAnimation;

        [Inject]
        private void Construct(IPhotoProvider photoProvider, IAnimalInPhotoProvider animalInPhotoProvider, PlayerController playerController)
        {
            _photoProvider = photoProvider;
            _playerController = playerController;
            _animalInPhotoProvider = animalInPhotoProvider;
        }

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _image.gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;

            _photoProvider.OnPhotoAndScoreReady += RenderImageAndScore;
        }

        private void OnDestroy()
        {
            _photoProvider.OnPhotoAndScoreReady -= RenderImageAndScore;
            DOTween.Kill(_image.rectTransform);
            DOTween.Kill(_canvasGroup);
        }

        private void RenderImageAndScore(Texture2D texture, PhotoScore score)
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
            _currentAnimation = StartCoroutine(ShowFlashReveal(texture, score, _animalInPhotoProvider.LastPhotoData));
        }

        private IEnumerator ShowFlashReveal(Texture2D texture, PhotoScore score, CapturedPhotoData photoData)
        {
            _image.texture = texture;
            _image.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            
            _playerController.ShakeCamera(0.3f, 1f);
            
            if (_scoreText != null)
            {
                _scoreText.text = $"\n{photoData.AnimalType}, {photoData.AnimalState.StateType}" +
                                  $"\nОчки: {score.TotalScore}" +
                                  $"\nРазмер: {score.SizePoints}" +
                                  $"\nПоложение: {score.PlacementPoints}" +
                                  $"\nМножитель редкости: {score.PoseMultiplier}";
                _scoreText.gameObject.SetActive(true);
            }
            
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

            _image.rectTransform.localScale = Vector3.one * 0.95f;

            Sequence reveal = DOTween.Sequence();
            reveal.Append(flashCg.DOFade(0f, 0.4f).SetEase(Ease.InSine));
            reveal.Join(_canvasGroup.DOFade(1f, 0.6f).SetEase(Ease.OutSine));
            reveal.Join(_image.rectTransform.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

            yield return reveal.WaitForCompletion();

            Destroy(flash);

            yield return new WaitForSeconds(_displayTime);

            yield return _canvasGroup.DOFade(0f, _fadeOutDuration).WaitForCompletion();

            _image.gameObject.SetActive(false);
            _scoreText.gameObject.SetActive(false);
            _currentAnimation = null;
        }
    }
}
