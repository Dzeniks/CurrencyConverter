namespace CurrencyConverter;

using CurrencyConverter.Enums;

public interface ICurrencyConverter
{
    DateTime ValidityDate { get; } 
    IEnumerable<CurrencyCode> SupportedCurrencies { get; }
    
    decimal Convert(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode, decimal amount);

    decimal ConversionRate(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode);

}