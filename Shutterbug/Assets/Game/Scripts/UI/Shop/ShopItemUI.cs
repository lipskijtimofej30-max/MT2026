using Game.Scripts.Data;
using Game.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Button _button;
        private Item _item;
        private ShopService _shopService;

        public void Initialize(Item item, ShopService shopService)
        {
            _item = item;
            _shopService = shopService;
            _infoText.text = $"{item.Name}\n" +
                             $"{item.Description}\n" +
                             $"Цена {item.Cost}";
            _button.onClick.AddListener(() => _shopService.BuyItem(_item));
        }
        
        private void Update()
        {
            _button.interactable = _shopService.CanBuyItem(_item);
        }
    }
}