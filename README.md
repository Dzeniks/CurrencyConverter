# Currency Converter Project

## Overview

This is a school project for creating a currency converter using C# programming language in console. The currency converter will utilize a RESTful Exchange API to fetch real-time exchange rates and also make use of CSV data from České národní banky (Czech National Bank) for historical exchange rate information.

## Features

1. **Real-time Currency Conversion:**
   - Fetch current exchange rates from a RESTful Exchange API.
   - Convert between different currencies based on the latest available rates.
   - 
2. **Error Handling:**
   - Implement robust error handling to manage unexpected scenarios, such as API failures or invalid user input.

## Prerequisites

Before running the application, make sure you have the following installed:

- [Visual Studio](https://visualstudio.microsoft.com/) or any C# compatible IDE.
- .NET Core SDK

## Is needed to add ExchangeAPI key to appsettings.json

```json
{
  "settings": {
    "apiKey" : "EXCHANGE_API_KEY"
  }
}
```
