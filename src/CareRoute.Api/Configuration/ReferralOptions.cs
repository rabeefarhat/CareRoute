// src/CareRoute.Api/Configuration/ReferralOptions.cs   (Api layer for now → Application in L08 · Referrals context)
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Configuration;

public sealed class ReferralOptions
{
    public const string SectionName = "Referrals";

    [Range(1, 168)]
    public int UrgentTriageHours { get; set; }

    [Range(1, 365)]
    public int MaxDraftAgeDays { get; set; }

    [Range(1, 30)]
    public int UrgentBookingDays { get; set; }

    [Range(1, 365)]
    public int RoutineBookingDays { get; set; }
}