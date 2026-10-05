// tests/CareRoute.Application.Tests/Infrastructure/InMemoryPatientStoreTests.cs
using CareRoute.Infrastructure.PatientRegistry;
using CareRoute.Testing.Fakers;
using FluentAssertions;

namespace CareRoute.Application.Tests.Infrastructure;

public sealed class InMemoryPatientStoreTests
{
    [Fact]
    public async Task Add_then_get_returns_the_same_patient()
    {
        // Arrange
        var store = new InMemoryPatientStore();
        var patient = new PatientFaker().Generate();                          // ①

        // Act
        await store.AddAsync(patient, CancellationToken.None);
        var found = await store.GetByIdAsync(patient.Id, CancellationToken.None);

        // Assert
        found.Should().Be(patient);                                           // ②
    }

    [Fact]
    public async Task Get_with_unknown_id_returns_null()
    {
        // Arrange
        var store = new InMemoryPatientStore();
        var unknownId = Guid.NewGuid();                                       // ③

        // Act
        var found = await store.GetByIdAsync(unknownId, CancellationToken.None);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task Concurrent_adds_are_all_stored()
    {
        // Arrange
        var store = new InMemoryPatientStore();
        var countBefore = (await store.GetAllAsync(CancellationToken.None)).Count; // ④
        var patients = new PatientFaker().Generate(100);                      // ⑤

        // Act
        await Parallel.ForEachAsync(patients, async (patient, ct) =>          // ⑥
            await store.AddAsync(patient, ct));

        // Assert
        var all = await store.GetAllAsync(CancellationToken.None);
        all.Should().HaveCount(countBefore + 100);                            // ⑦
        all.Should().Contain(patients);
    }
}