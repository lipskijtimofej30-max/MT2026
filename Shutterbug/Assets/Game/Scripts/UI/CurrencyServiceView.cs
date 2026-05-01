using System;
using Game.Service.Currency;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
    public class CurrencyServiceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        private ICurrencyService _currencyService;

        [Inject]
        private void Construct(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
            RenderText(_currencyService.Currency);
            _currencyService.OnCurrencyChanged += RenderText;
        }

        private void OnDestroy()
        {
            _currencyService.OnCurrencyChanged -= RenderText;
        }

        private void RenderText(int amount)
        {
            _moneyText.text =$"Reputation: {amount}";
        }

    }
}