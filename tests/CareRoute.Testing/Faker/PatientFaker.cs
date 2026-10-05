// tests/CareRoute.Testing/Fakers/PatientFaker.cs
using Bogus;
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Testing.Fakers;

public sealed class PatientFaker : Faker<Patient>
{
    public const int DefaultSeed = 20260;
    private static readonly DateTime ReferenceDate = CareRouteDataSet.DefaultReferenceDate;

    public PatientFaker(int seed = DefaultSeed) : base("nl_BE")
    {
        UseSeed(seed);
        CustomInstantiator(f => Patient.Register(
            new PatientId(f.Random.Guid()),
            NationalNumber.Create(f.CareRoute().NationalNumber(ReferenceDate)),
            PersonName.Create(f.Name.FirstName(), f.Name.LastName()),
            new DoctorId(f.Random.Guid())));
    }
}