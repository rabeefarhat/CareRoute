// tests/CareRoute.Domain.Tests/PatientRegistry/NationalNumberTests.cs
using CareRoute.Domain.PatientRegistry;
using CareRoute.Domain.SharedKernel;
using FluentAssertions;

namespace CareRoute.Domain.Tests.PatientRegistry;

public class NationalNumberTests
{
    [Theory]
    [InlineData("85073003328", "1985-07-30")]      // ① typical 1900s number
    [InlineData("00010100105", "2000-01-01")]      // ② 2000+ rule (prefix 2)
    [InlineData("00010100173", "1900-01-01")]      // ③ same digits, other check -> other century
    [InlineData("99123199940", "1999-12-31")]      // ④ last day of the 1900s
    [InlineData("99123199969", "2099-12-31")]      // ⑤ largest base: 2,991,231,999 > int.MaxValue
    [InlineData("00022900145", "2000-02-29")]      // ⑥ leap day in a leap year
    [InlineData("85.07.30-033.28", "1985-07-30")]  // ⑦ dots and dashes
    [InlineData("85 07 30 033 28", "1985-07-30")]  //    spaces
    public void Valid_numbers_are_accepted_and_the_birth_date_is_derived(string raw, string expectedBirthDate)
    {
        var number = NationalNumber.Create(raw);

        number.BirthDate.Should().Be(DateOnly.Parse(expectedBirthDate));
    }

    [Theory]
    [InlineData(null, "national_number.required")]
    [InlineData("", "national_number.required")]
    [InlineData("   ", "national_number.required")]
    [InlineData("8507300332", "national_number.format")]          // 10 digits
    [InlineData("850730033280", "national_number.format")]        // 12 digits
    [InlineData("85O73003328", "national_number.format")]         // letter O instead of zero
    [InlineData("85073003329", "national_number.invalid_check")]  // check off by one
    [InlineData("85073003228", "national_number.invalid_check")]  // one typo in the serial
    [InlineData("01022900166", "national_number.invalid_birth_date")] // correct check, but 2001-02-29 doesn't exist
    public void Invalid_numbers_are_rejected_with_a_stable_code(string? raw, string expectedCode)
    {
        Action act = () => NationalNumber.Create(raw);

        act.Should().Throw<DomainException>().Which.Code.Should().Be(expectedCode);
    }

    [Fact]
    public void Separators_are_removed_from_the_stored_value()
    {
        var number = NationalNumber.Create("85.07.30-033.28");

        number.Value.Should().Be("85073003328");
        number.Formatted.Should().Be("85.07.30-033.28");
    }

    [Fact]
    public void Leading_zeros_are_kept()
    {
        NationalNumber.Create("00010100105").Value.Should().Be("00010100105").And.HaveLength(11);
    }

    [Fact]
    public void Numbers_with_the_same_digits_are_equal_whatever_the_input_format()
    {
        var withSeparators = NationalNumber.Create("85.07.30-033.28");
        var plain = NationalNumber.Create("85073003328");

        withSeparators.Should().Be(plain);
        (withSeparators == plain).Should().BeTrue();
        withSeparators.GetHashCode().Should().Be(plain.GetHashCode());
    }

    [Fact]
    public void ToString_masks_the_birth_date_and_serial()
    {
        NationalNumber.Create("85073003328").ToString().Should().Be("85*******28");
    }
}