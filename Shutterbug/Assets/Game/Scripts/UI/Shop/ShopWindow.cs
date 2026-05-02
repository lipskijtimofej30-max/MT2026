using DG.Tweening;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Service;
using Game.Scripts.UI.Shop;
using Game.Service;
using UnityEngine;
using Zenject;

public class ShopWindow : TabletWindow
{
    [SerializeField] private ShopItemUI _itemPrefab;
    [SerializeField] private ShopCardUI _cardPrefab;
    [SerializeField] private RectTransform _windowRoot;
    [SerializeField] private Transform _content;

    [Header("Animation Settings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _showDuration = 0.4f;
    [SerializeField] private float _hideDuration = 0.3f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;
    
    private StatUpgradesDatabase _configs;
    private ItemDatabase _itemDatabase;
    private IProgressionService _progressionService;
    private ShopService _shopService;
    private SignalBus _signalBus;
    
    private Coroutine _currentAnimationCoroutine;
    private bool _isOpen;
    public bool IsItem { get; set; }

    [Inject]
    private void Construct(StatUpgradesDatabase configs, IProgressionService progressionService,
        ShopService shopService, ItemDatabase itemDatabase, SignalBus signalBus)
    {
        _configs = configs;
        _progressionService = progressionService;
        _shopService = shopService;
        _itemDatabase = itemDatabase;
        _signalBus = signalBus;
    }


    public override void ToggleWindow()
    {
        if (_isOpen) Open();
        else Close();
    }

    private void Close()
    {
        _isOpen = false;

        if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
        DOTween.Kill(_windowRoot);
        if(gameObject.activeInHierarchy)
            _currentAnimationCoroutine = StartCoroutine(HideAnimation(_windowRoot, _canvasGroup, _hideDuration, _hideEase));
    }

    private void Open()
    {
        _isOpen = true;
        _canvasGroup.alpha = 0f;
        _windowRoot.localScale = Vector3.zero;
        _windowRoot.gameObject.SetActive(true);
        
        RefreshWindow();
        
        if (_currentAnimationCoroutine != null) StopCoroutine(_currentAnimationCoroutine);
        DOTween.Kill(_windowRoot);
        if(gameObject.activeInHierarchy)
            _currentAnimationCoroutine = StartCoroutine(ShowAnimation(_windowRoot, _canvasGroup, _showDuration, _showEase));
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
            RefreshWindow();
        }
        else
        {
            _canvasGroup.alpha = 0f;
            _windowRoot.localScale = Vector3.zero;
        }
    }
    
    public void RefreshWindow()
    {
        foreach (Transform child in _content) 
            Destroy(child.gameObject);

        if (!IsItem)
        {
            foreach (var statConfig in _configs.Databased)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Initialize(statConfig, _progressionService, _shopService, _signalBus);
            }
        }
        else
        {
            foreach (var item in _itemDatabase.Databased)
            {
                var card = Instantiate(_itemPrefab, _content);
                card.Initialize(item, _shopService);
            }   
        }
    }
}
