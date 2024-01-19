using CurrencyConverter.Exceptions;
using CurrencyConverter.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace CurrencyConverter;

using Enums;

public class ExchangeRateApiConverter : ICurrencyConverter
{
    private string ApiKey { get; init; }
    
    private Dictionary<CurrencyCode, decimal> _rates;
    
    public DateTime ValidityDate { get; private set; }

    public IEnumerable<CurrencyCode> SupportedCurrencies => _rates.Keys;

    public ExchangeRateApiConverter(string apiKey)
    {
        ApiKey = apiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new CurrencyException("API key is empty");
        }
        var restClient = new RestClient(
            $"https://v6.exchangerate-api.com/v6/{apiKey}/",
            configureSerialization: s => s.UseNewtonsoftJson()
        );
        var request = new RestRequest("latest/EUR");
        var response = restClient.GetAsync(request).Result;
        
        if (response.IsSuccessful && response.Content != null)
        {
            var result = JsonConvert.DeserializeObject<ExchangeApiModel>(response.Content);
            if (result == null)
            {
                throw new CurrencyException("API error");
            }
            
            _rates = new Dictionary<CurrencyCode, decimal>();
            foreach (var rate in result.ConversionRates)
            {
                if (Enum.TryParse<CurrencyCode>(rate.Key, out var currencyCode))
                {
                    _rates.Add(currencyCode, rate.Value);
                }
            }
            ValidityDate = result.TimeLastUpdateUtcUpdateUtc;
        }
        else
        {
            throw new CurrencyException("API error");
        }
    }
    
    public decimal Convert(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode, decimal amount)
    {
        return amount * ConversionRate(baseCurrencyCode, targetCurrencyCode);
    }
    
    public decimal ConversionRate(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode)
    {
        return (1 / _rates[baseCurrencyCode]) * _rates[targetCurrencyCode];
    }
}