using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CurrencyConverter.Enums;
using CurrencyConverter.Models;
using CurrencyConverter.Exceptions;

namespace CurrencyConverter;

public class CnbCurrencyConverter : ICurrencyConverter
{
    public DateTime ValidityDate { get; private set; }
    public IEnumerable<CurrencyCode> SupportedCurrencies { get; private set; }

    public decimal Convert(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode, decimal amount)
    {
        return amount * ConversionRate(baseCurrencyCode, targetCurrencyCode);
    }
    
    public decimal ConversionRate(CurrencyCode baseCurrencyCode, CurrencyCode targetCurrencyCode)
    {
        if (!SupportedCurrencies.Contains(baseCurrencyCode) || !SupportedCurrencies.Contains(targetCurrencyCode))
        {
            throw new CurrencyException("Currency not supported");
        }
        return _rates[baseCurrencyCode] / _rates[targetCurrencyCode];
    }

    private Dictionary<CurrencyCode, decimal> _rates;

    public CnbCurrencyConverter()
    {
        Refresh();
    }

    public void Refresh()
    {
        try
        {
            using (var client = new HttpClient())
            {
                // Fetch string from
                // "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt"
                using (var response = client
                           .GetAsync(
                               "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/" +
                               "kurzy-devizoveho-trhu/denni_kurz.txt")
                           .Result)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    var csvConfig = new CsvConfiguration(new CultureInfo("cs-CZ"))
                    {
                        Delimiter = "|",
                        ShouldSkipRecord = record => record.Row.Parser.Row < 2,
                    };
                    // Parse string to CSV
                    using (var csv = new CsvReader(new StringReader(responseBody), csvConfig))
                    {
                        var records = csv.GetRecords<CNBmodel>();

                        _rates = new Dictionary<CurrencyCode, decimal>();

                        foreach (var record in records)
                        {
                            var rate = record.Rate / record.Amount;
                            _rates.Add(record.CurrencyCode, rate);
                        }

                        // CZK is not in the CSV file, so we add it manually
                        _rates.Add(CurrencyCode.CZK, 1);

                        SupportedCurrencies = _rates.Keys;
                    }

                    string date = responseBody.Substring(0, 10);
                    ValidityDate = DateTime.ParseExact(date, "dd.MM.yyyy", null)
                        .AddHours(14).AddMinutes(30);
                }
            }
        }
        catch (Exception e)
        {
            throw new CurrencyException("Failed to refresh currency rate", e);
        }
    }
}