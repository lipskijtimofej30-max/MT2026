using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
    public class ButtonHandler : MonoBehaviour
    {
        [SerializeField] private ShopWindow _shopWindow;
        [SerializeField] private Button _button;
        [SerializeField] private bool _isItemWindow;

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _shopWindow.IsItem = _isItemWindow;
            _shopWindow.RefreshWindow();
        }
    }
}