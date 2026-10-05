// tests/CareRoute.Domain.Tests/DomainDesignRulesTests.cs
using System.Reflection;
using CareRoute.Domain.PatientRegistry;
using FluentAssertions;

namespace CareRoute.Domain.Tests;

public class DomainDesignRulesTests
{
    [Fact]
    public void No_domain_type_has_a_public_setter()
    {
        var domainAssembly = typeof(Patient).Assembly;                                   // ①

        var publicSetters = domainAssembly.GetTypes()
            .Where(t => t.IsPublic)
            .SelectMany(t => t.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))   // ②
            .Where(p => p.SetMethod is { IsPublic: true })                               // ③
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name}")
            .ToList();

        publicSetters.Should().BeEmpty("domain state may only change through methods that enforce invariants");
    }
}