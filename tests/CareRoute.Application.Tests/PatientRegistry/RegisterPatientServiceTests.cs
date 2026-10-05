// tests/CareRoute.Application.Tests/PatientRegistry/RegisterPatientServiceTests.cs
using CareRoute.Application.PatientRegistry;
using CareRoute.Domain.PatientRegistry;
using CareRoute.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;

namespace CareRoute.Application.Tests.PatientRegistry;

public class RegisterPatientServiceTests
{
    private const string ValidNationalNumber = "85.07.30-033.28";
    private static readonly Guid GpId = Guid.Parse("d0c70000-0000-0000-0000-000000000001");

    private readonly IPatientStore _store = Substitute.For<IPatientStore>();

    [Fact]
    public async Task Registering_adds_the_patient_to_the_store_exactly_once()
    {
        var service = new RegisterPatientService(_store);

        await service.RegisterAsync("Emma", "Willems", ValidNationalNumber, GpId, CancellationToken.None);

        await _store.Received(1).AddAsync(
            Arg.Is<Patient>(p => p.NationalNumber.Value == "85073003328" && p.GpId == new DoctorId(GpId)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registering_returns_the_id_of_the_stored_patient()
    {
        Patient? stored = null;
        await _store.AddAsync(Arg.Do<Patient>(p => stored = p), Arg.Any<CancellationToken>());
        var service = new RegisterPatientService(_store);

        var id = await service.RegisterAsync("Emma", "Willems", ValidNationalNumber, GpId, CancellationToken.None);

        stored.Should().NotBeNull();
        id.Should().Be(stored!.Id);
        id.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public async Task An_invalid_national_number_never_reaches_the_store()
    {
        var service = new RegisterPatientService(_store);

        var act = () => service.RegisterAsync("Emma", "Willems", "85.07.30-033.29", GpId, CancellationToken.None);

        (await act.Should().ThrowAsync<DomainException>()).Which.Code.Should().Be("national_number.invalid_check");
        await _store.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }
}