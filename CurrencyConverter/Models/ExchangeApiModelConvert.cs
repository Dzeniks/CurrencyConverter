using Newtonsoft.Json;

namespace CurrencyConverter.Models;

public class ExchangeApiModelConvert
{
    [JsonProperty("time_last_update_utc")] public DateTime TimeLastUpdateUtcUpdateUtc { get; init; }

    [JsonProperty("conversion_result")] public decimal ConversionResult { get; init; }

}