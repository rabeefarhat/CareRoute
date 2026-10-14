// src/CareRoute.Api/Configuration/ReferralPolicyController.cs   (Api layer · Referrals context)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CareRoute.Api.Configuration;

[ApiController]
[Route("api/referral-policy")]
public sealed class ReferralPolicyController(IOptions<ReferralOptions> options) : ControllerBase
{
    [HttpGet]
    public ActionResult<ReferralOptions> Get() => Ok(options.Value);
}