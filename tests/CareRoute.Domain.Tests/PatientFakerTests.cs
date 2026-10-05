// tests/CareRoute.Domain.Tests/PatientFakerTests.cs
using CareRoute.Testing.Fakers;
using FluentAssertions;

namespace CareRoute.Domain.Tests;

public sealed class PatientFakerTests
{
    [Fact]
    public void Same_seed_produces_the_same_first_patient()
    {
        // Arrange
        var first = new PatientFaker(seed: 42);
        var second = new PatientFaker(seed: 42);

        // Act
        var a = first.Generate();
        var b = second.Generate();

        // Assert
        b.Should().Be(a);                                                     // ①
    }

    [Fact]
    public void Different_seeds_produce_different_patients()
    {
        // Act
        var a = new PatientFaker(seed: 1).Generate();
        var b = new PatientFaker(seed: 2).Generate();

        // Assert
        b.Id.Should().NotBe(a.Id);                                            // ②
    }

    [Fact]
    public void Birth_dates_are_within_90_years_before_the_reference_date()
    {
        // Arrange
        var oldestAllowed = new DateOnly(1936, 1, 1);                         // ③
        var newestAllowed = new DateOnly(2026, 1, 1);

        // Act
        var birthDates = new PatientFaker().Generate(200).Select(p => p.DateOfBirth);

        // Assert
        birthDates.Should().OnlyContain(d => d >= oldestAllowed && d <= newestAllowed); // ④
    }
}