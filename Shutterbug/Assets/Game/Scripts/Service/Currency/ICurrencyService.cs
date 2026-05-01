using System;

namespace Game.Service.Currency
{
    public interface ICurrencyService
    {
        int Currency { get; }
        void AddCurrency(int value);
        void RemoveCurrency(int value);
        event Action<int> OnCurrencyChanged;
    }
}