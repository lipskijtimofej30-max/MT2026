using System;
using UnityEngine;

namespace Game.Service.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private int _money;
        public int Currency => _money;
        
        public event Action<int> OnCurrencyChanged;

        public void AddCurrency(int value)
        {
            if(value < 0 ) return;
            _money += value;
            Debug.LogWarning($"Add money {value}, money {_money}");
            OnCurrencyChanged?.Invoke(_money);
        }

        public void RemoveCurrency(int value)
        {
            if (_money >= value)
            {
                _money -= value;
                Debug.LogWarning($"Remove money {value}, money {_money}");
                OnCurrencyChanged?.Invoke(_money);
            }
        }
    }
}