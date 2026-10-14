// src/CareRoute.Api/Program.cs   (Api layer · composition root)
using CareRoute.Api.Configuration;
using CareRoute.Api.Errors;
using CareRoute.Api.Patients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>                                             // ①
    {
        var problem = context.ProblemDetails;
        var status = problem.Status ?? context.HttpContext.Response.StatusCode;
        var (type, code) = ProblemTypes.ForStatus(status);                                   // ②

        if (problem.Type is null || !problem.Type.StartsWith(ProblemTypes.BaseUri, StringComparison.Ordinal))
        {
            problem.Type = type;                                                             // ③
        }

        problem.Extensions.TryAdd("errorCode", code);                                        // ④
        problem.Instance ??= context.HttpContext.Request.Path;                               // ⑤
    });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IPatientStore, InMemoryPatientStore>();

builder.Services.AddOptions<ReferralOptions>()
    .BindConfiguration(ReferralOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();                                                                    // ⑥

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();