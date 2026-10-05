// tests/CareRoute.Application.Tests/Infrastructure/InMemoryPatientStoreTests.cs
using CareRoute.Domain.PatientRegistry;
using CareRoute.Infrastructure.PatientRegistry;
using CareRoute.Testing.Fakers;
using FluentAssertions;

namespace CareRoute.Application.Tests.Infrastructure;

public class InMemoryPatientStoreTests
{
    private const int SeededPatients = 3;

    [Fact]
    public async Task Add_then_get_returns_the_same_patient()
    {
        var store = new InMemoryPatientStore();
        var patient = new PatientFaker().Generate();

        await store.AddAsync(patient, CancellationToken.None);
        var found = await store.GetByIdAsync(patient.Id, CancellationToken.None);

        found.Should().BeSameAs(patient);
    }

    [Fact]
    public async Task Get_with_unknown_id_returns_null()
    {
        var store = new InMemoryPatientStore();

        var found = await store.GetByIdAsync(PatientId.New(), CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task Concurrent_adds_are_all_stored()
    {
        var store = new InMemoryPatientStore();
        var patients = new PatientFaker().Generate(100);

        await Task.WhenAll(patients.Select(p => Task.Run(() => store.AddAsync(p, CancellationToken.None))));

        var all = await store.GetAllAsync(CancellationToken.None);
        all.Should().HaveCount(100 + SeededPatients);
    }
}