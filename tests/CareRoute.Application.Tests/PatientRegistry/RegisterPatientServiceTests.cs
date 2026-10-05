// tests/CareRoute.Application.Tests/PatientRegistry/RegisterPatientServiceTests.cs
using CareRoute.Application.PatientRegistry;
using CareRoute.Domain.PatientRegistry;
using FluentAssertions;
using NSubstitute;

namespace CareRoute.Application.Tests.PatientRegistry;

public sealed class RegisterPatientServiceTests
{
    private static readonly DateOnly BirthDate = new(1984, 3, 12);            // ①

    [Fact]
    public async Task Registering_adds_the_patient_to_the_store_exactly_once()
    {
        // Arrange
        var store = Substitute.For<IPatientStore>();                          // ②
        var service = new RegisterPatientService(store);

        // Act
        await service.RegisterAsync("Lotte", "Peeters", BirthDate, CancellationToken.None);

        // Assert
        await store.Received(1).AddAsync(                                     // ③
            Arg.Is<Patient>(p =>
                p.FirstName == "Lotte" &&
                p.LastName == "Peeters" &&
                p.DateOfBirth == BirthDate),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registering_returns_the_id_of_the_stored_patient()
    {
        // Arrange
        var store = Substitute.For<IPatientStore>();
        Patient? stored = null;
        store.When(s => s.AddAsync(Arg.Any<Patient>(), Arg.Any<CancellationToken>()))
             .Do(call => stored = call.Arg<Patient>());                       // ④
        var service = new RegisterPatientService(store);

        // Act
        var id = await service.RegisterAsync("Jens", "Maes", BirthDate, CancellationToken.None);

        // Assert
        stored.Should().NotBeNull();
        id.Should().NotBeEmpty();                                             // ⑤
        id.Should().Be(stored!.Id);                                           // ⑥
    }
}