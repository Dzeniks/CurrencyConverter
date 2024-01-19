using Newtonsoft.Json;

namespace CurrencyConverter.Models;

public class ExchangeApiModelSupportedCodes
{
    [JsonProperty("supported_codes")] public required IEnumerable<IEnumerable<string>> SupportedCodes { get; init; }
}