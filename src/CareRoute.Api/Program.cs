// src/CareRoute.Api/Program.cs   (Api layer · composition root)
using CareRoute.Api.Configuration;
using CareRoute.Api.Errors;
using CareRoute.Api.Patients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();                                          // ①
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();                // ②
builder.Services.AddHealthChecks();                                            // ③

builder.Services.AddSingleton<IPatientStore, InMemoryPatientStore>();          // ④

builder.Services.AddOptions<ReferralOptions>()
    .BindConfiguration(ReferralOptions.SectionName)                            // ⑤
    .ValidateDataAnnotations()
    .ValidateOnStart();                                                        // ⑥

var app = builder.Build();

app.UseExceptionHandler();                                                     // ⑦

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health");                                                // ⑧
app.MapControllers();

app.Run();