// src/CareRoute.Api/Program.cs  (composition root)
using CareRoute.Api.ErrorHandling;
using CareRoute.Api.Options;
using CareRoute.Application;
using CareRoute.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (DI registrations) ----
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();                                   // ①
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();         // ②
builder.Services
    .AddApplication()
    .AddInfrastructure();   // ③
builder.Services.AddOptions<ReferralOptions>()
    .BindConfiguration(ReferralOptions.Section)                         // ④
    .ValidateDataAnnotations()
    .ValidateOnStart();                                                 // ⑤
builder.Services.AddHealthChecks();                                     // ⑥

var app = builder.Build();

// ---- Pipeline (order = behaviour) ----
app.UseExceptionHandler();                                              // ⑦ outermost: catches everything below

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");                                         // ⑧

app.Run();