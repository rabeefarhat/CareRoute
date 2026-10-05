// src/CareRoute.Domain/SharedKernel/DomainException.cs
namespace CareRoute.Domain.SharedKernel;

public sealed class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}