using Newtonsoft.Json;

namespace CurrencyConverter.Models;

public class ExchangeApiModelRate
{
    [JsonProperty("time_last_update_utc")] public DateTime TimeLastUpdateUtcUpdateUtc { get; init; }

    [JsonProperty("conversion_rate")] public decimal ConversionRates { get; init; }

}