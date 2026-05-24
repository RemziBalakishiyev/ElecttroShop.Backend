namespace ElectroShop.Domain.Exceptions;

/// <summary>
/// Optimistic Concurrency Control konflikti
/// RowVersion uyğun gəlmir - başqa istifadəçi dəyişiklik etmişdir
/// </summary>
public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}



