namespace CushyPay.Domain.Exceptions;

public class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException(string message) : base(message)
    {
    }
}

