namespace CurrencyConverter.Exceptions;

public class CurrencyException : Exception
{
    public CurrencyException ()
    {}

    public CurrencyException (string message, Exception innerException = null)
        : base (message, innerException)
    {}    
}