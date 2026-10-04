// src/CareRoute.Api/Options/ReferralOptions.cs
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Options;

public sealed class ReferralOptions
{
    public const string Section = "Referrals";

    [Range(1, 365)]
    public int MaxDraftAgeDays { get; init; }

    [Range(1, 168)]
    public int UrgentTriageHours { get; init; }
}