// tests/CareRoute.Domain.Tests/PatientRegistry/PatientTests.cs
using CareRoute.Domain.PatientRegistry;
using CareRoute.Domain.SharedKernel;
using FluentAssertions;

namespace CareRoute.Domain.Tests.PatientRegistry;

public class PatientTests
{
    private static readonly NationalNumber Number = NationalNumber.Create("85073003328");
    private static readonly PersonName Name = PersonName.Create("Emma", "Willems");
    private static readonly DoctorId Gp = new(Guid.Parse("d0c70000-0000-0000-0000-000000000001"));
    private static readonly DoctorId OtherGp = new(Guid.Parse("d0c70000-0000-0000-0000-000000000002"));

    [Fact]
    public void Register_sets_the_id_number_name_and_gp()
    {
        var id = PatientId.New();

        var patient = Patient.Register(id, Number, Name, Gp);

        patient.Id.Should().Be(id);
        patient.NationalNumber.Should().Be(Number);
        patient.Name.Should().Be(Name);
        patient.GpId.Should().Be(Gp);
        patient.DateOfBirth.Should().Be(new DateOnly(1985, 7, 30));
    }

    [Fact]
    public void Register_without_a_gp_is_rejected()
    {
        Action act = () => Patient.Register(PatientId.New(), Number, Name, new DoctorId(Guid.Empty));

        act.Should().Throw<DomainException>().Which.Code.Should().Be("patient.gp_required");
    }

    [Fact]
    public void Register_with_a_default_patient_id_is_rejected()
    {
        Action act = () => Patient.Register(default, Number, Name, Gp);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("patient.id_required");
    }

    [Fact]
    public void Register_with_a_null_name_is_a_programming_error()
    {
        Action act = () => Patient.Register(PatientId.New(), Number, null!, Gp);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReassignGp_changes_the_gp()
    {
        var patient = Patient.Register(PatientId.New(), Number, Name, Gp);

        patient.ReassignGp(OtherGp);

        patient.GpId.Should().Be(OtherGp);
    }

    [Fact]
    public void ReassignGp_to_an_empty_id_is_rejected_and_keeps_the_current_gp()
    {
        var patient = Patient.Register(PatientId.New(), Number, Name, Gp);

        Action act = () => patient.ReassignGp(default);

        act.Should().Throw<DomainException>().Which.Code.Should().Be("patient.gp_required");
        patient.GpId.Should().Be(Gp);
    }

    [Fact]
    public void Two_patients_with_the_same_id_are_equal_even_if_their_names_differ()
    {
        var id = PatientId.New();
        var original = Patient.Register(id, Number, Name, Gp);
        var renamed = Patient.Register(id, Number, PersonName.Create("Emma", "Willems-Peeters"), Gp);

        original.Should().Be(renamed);
        (original == renamed).Should().BeTrue();
    }
}