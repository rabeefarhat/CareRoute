// src/CareRoute.Api/Program.cs  (composition root)
using CareRoute.Api.ErrorHandling;
using CareRoute.Api.Options;
using CareRoute.Application;
using CareRoute.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (DI registrations) ----
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>(); 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();         
builder.Services
    .AddApplication()
    .AddInfrastructure();   // ③
builder.Services.AddOptions<ReferralOptions>()
    .BindConfiguration(ReferralOptions.Section)                         
    .ValidateDataAnnotations()
    .ValidateOnStart();                                                 
builder.Services.AddHealthChecks();                                     

var app = builder.Build();

// ---- Pipeline (order = behaviour) ----
app.UseExceptionHandler();                                              

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");                                         

app.Run();