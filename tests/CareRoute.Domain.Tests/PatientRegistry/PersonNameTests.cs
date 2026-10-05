// tests/CareRoute.Domain.Tests/PatientRegistry/PersonNameTests.cs
using CareRoute.Domain.PatientRegistry;
using CareRoute.Domain.SharedKernel;
using FluentAssertions;

namespace CareRoute.Domain.Tests.PatientRegistry;

public class PersonNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void A_blank_first_name_is_rejected(string? first)
    {
        Action act = () => PersonName.Create(first, "Willems");

        act.Should().Throw<DomainException>().Which.Code.Should().Be("person_name.first_required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t ")]
    public void A_blank_last_name_is_rejected(string? last)
    {
        Action act = () => PersonName.Create("Emma", last);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("person_name.last_required");
    }

    [Fact]
    public void Names_are_trimmed()
    {
        var name = PersonName.Create("  Emma ", " Willems  ");

        name.First.Should().Be("Emma");
        name.Last.Should().Be("Willems");
        name.Should().Be(PersonName.Create("Emma", "Willems"));
    }

    [Fact]
    public void A_name_of_exactly_the_maximum_length_is_accepted()
    {
        var act = () => PersonName.Create(new string('a', PersonName.MaxLength), "Willems");

        act.Should().NotThrow();
    }

    [Fact]
    public void A_name_one_character_too_long_is_rejected()
    {
        Action act = () => PersonName.Create(new string('a', PersonName.MaxLength + 1), "Willems");

        act.Should().Throw<DomainException>().Which.Code.Should().Be("person_name.too_long");
    }
}