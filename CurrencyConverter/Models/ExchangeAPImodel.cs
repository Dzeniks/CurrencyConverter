using Newtonsoft.Json;

namespace CurrencyConverter.Models;

public class ExchangeApiModel
{
    [JsonProperty("time_last_update_utc")] public DateTime TimeLastUpdateUtcUpdateUtc { get; init; }

    [JsonProperty("conversion_rates")] public Dictionary<string, decimal> ConversionRates { get; init; } = new();

}