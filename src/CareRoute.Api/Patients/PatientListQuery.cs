// src/CareRoute.Api/Patients/PatientListQuery.cs   (Api layer · Patient Registry)
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Patients;

public sealed record PatientListQuery
{
    public const int MaxPageSize = 100;
    public const int MaxPage = 10_000;

    [Range(1, MaxPage)]
    public int Page { get; init; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; init; } = 20;

    [StringLength(100)]
    public string? Search { get; init; }
}