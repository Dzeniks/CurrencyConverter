using CurrencyConverter.Exceptions;
using CurrencyConverter.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace CurrencyConverter;

using Enums;

public class ExchangeRateApiOnlineConverter : ICurrencyConverter
{
    private string _apiKey { get; init; }
    // If validity date is not set, it means that the API has not been called yet
    public DateTime ValidityDate { get; private set; }

    public IEnumerable<CurrencyCode> SupportedCurrencies
    {
        get
        {
            var request = new RestRequest("codes");
            var response = _restClient.GetAsync(request).Result;
            if (!response.IsSuccessful || response.Content == null)
                throw new CurrencyException("API error - SupportedCurrencies");
            var result = JsonConvert.DeserializeObject<ExchangeApiModelSupportedCodes>(response.Content);
            if (result == null)
            {
                throw new CurrencyException("API error - SupportedCurrencies");
            }
            var currencyCodes = new List<CurrencyCode>();
            foreach (var currencyCode in result.SupportedCodes)
            {
                if (Enum.TryParse<CurrencyCode>(currencyCode.First(), out var code))
                {
                    currencyCodes.Add(code);
                }
            }
            return currencyCodes;
        }
    }

    private RestClient _restClient { get; init; }
    
    public ExchangeRateApiOnlineConverter(string apiKey)
    {
        _apiKey = apiKey;
        
        // Create restClient
        _restClient = new RestClient(
            $"https://v6.exchangerate-api.com/v6/{apiKey}/",
            configureSerialization: s => s.UseNewtonsoftJson()
        );
        
        
    }
    
    public decimal Convert(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode, decimal amount)
    {
        if (!SupportedCurrencies.Contains(baseCurrencyCode) || !SupportedCurrencies.Contains(targetCurrencyCode))
        {
            throw new CurrencyException("Currency not supported");
        }
        var request = new RestRequest($"pair/{baseCurrencyCode}/{targetCurrencyCode}/{amount}");
        var response = _restClient.GetAsync<ExchangeApiModelConvert>(request).Result;
        if (response == null)
        {
            throw new CurrencyException("API error - Convert");
        }
        ValidityDate = response.TimeLastUpdateUtcUpdateUtc;
        return response?.ConversionResult ?? 0;
    }

    
    public decimal ConversionRate(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode)
    {
        if (!SupportedCurrencies.Contains(baseCurrencyCode) || !SupportedCurrencies.Contains(targetCurrencyCode))
        {
            throw new CurrencyException("Currency not supported");
        }
        var request = new RestRequest($"pair/{baseCurrencyCode}/{targetCurrencyCode}/");
        var response = _restClient.GetAsync<ExchangeApiModelRate>(request).Result;
        if (response == null)
        {
            throw new CurrencyException("API error - ConversionRate");
        }
        ValidityDate = response.TimeLastUpdateUtcUpdateUtc;
        return response?.ConversionRates ?? 0;
    }
}