// See https://aka.ms/new-console-template for more information

using CurrencyConverter.Enums;
using Microsoft.Extensions.Configuration;

namespace CurrencyConverter;

public class Program
{
    public sealed class Settings
    {
        public required string apiKey { get; set; }
    }
    
    public static void Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // Get values from the config given their key and their target type.
        Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();
        
        var apiKey = settings?.apiKey;
        if (apiKey == null)
        {
            Console.WriteLine("API key not found");
            return;
        }
        
        CnbCurrencyConverter cnbConverter = new CnbCurrencyConverter();
        ExchangeRateApiConverter apiConverter = new ExchangeRateApiConverter(apiKey);
        ExchangeRateApiOnlineConverter apiOnlineConvertor = new ExchangeRateApiOnlineConverter(apiKey);
        var convertors = new Dictionary<string, ICurrencyConverter>();
        convertors.Add("1", cnbConverter);
        convertors.Add("2", apiConverter);
        convertors.Add("3", apiOnlineConvertor);
        // Test
        Console.WriteLine("Supported currencies:");
        foreach (var convertor in convertors)
        {
            Console.WriteLine(convertor.Key);
            Console.WriteLine("Number of supported currencies:");
            Console.WriteLine(convertor.Value.SupportedCurrencies.Count());
            Console.WriteLine("Validity date:");
            Console.WriteLine(convertor.Value.ValidityDate);
            Console.WriteLine();
        }

        foreach (var converter in convertors)
        {
            // Test all convertors with values
            Console.WriteLine($"Testing {converter.Key}");

            try
            {
                Console.WriteLine($"1 EUR to CZK = {converter.Value.Convert(CurrencyCode.EUR, CurrencyCode.CZK, 1)}");
                Console.WriteLine($"100 HUF to CZK = {converter.Value.Convert(CurrencyCode.HUF, CurrencyCode.CZK, 100)}");
                Console.WriteLine($"1 USD to EUR = {converter.Value.Convert(CurrencyCode.USD, CurrencyCode.GBP, 1)}");
                Console.WriteLine($"1 EUR to USD = {converter.Value.Convert(CurrencyCode.EUR, CurrencyCode.USD, 1)}");
                Console.WriteLine($"100 RUB to CZK = {converter.Value.Convert(CurrencyCode.RUB, CurrencyCode.CZK, 100)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine();
        }
        
        // User console app
        
         Console.WriteLine();
         Console.WriteLine("Choose converter:");
         Console.WriteLine("1. CNB");
         Console.WriteLine("2. ExchangeAPI");
         Console.WriteLine("3. ExchangeAPI Online");
         var userConverter = Console.ReadLine();
         // Check if converter is string
         if (userConverter != null && !convertors.ContainsKey(userConverter))
         {
             Console.WriteLine("Invalid converter");
             return;
         }
         var selectedConverter = convertors[userConverter];
         Console.WriteLine("Enter base currency code:");
         // Try parse
         if (!Enum.TryParse<CurrencyCode>(Console.ReadLine() ?? string.Empty, out var baseCurrencyCode))
         {
             Console.WriteLine("Invalid currency code");
             return;
         }
         Console.WriteLine("Enter target currency code:");
         if (!Enum.TryParse<CurrencyCode>(Console.ReadLine() ?? string.Empty, out var targetCurrencyCode))
         {
             Console.WriteLine("Invalid currency code");
             return;
         }
         
         if (!selectedConverter.SupportedCurrencies.Contains(targetCurrencyCode))
         {
             Console.WriteLine("Currency not supported");
             return;
         }
         Console.WriteLine("Enter amount:");
         var amount = decimal.Parse(Console.ReadLine() ?? string.Empty);
        
         try
         {
             Console.WriteLine("Converting...");
             var result = selectedConverter.Convert(baseCurrencyCode, targetCurrencyCode, amount);
             Console.WriteLine($"{amount} {baseCurrencyCode} = {result} {targetCurrencyCode}");
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
         }
    }
}