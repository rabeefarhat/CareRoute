// src/CareRoute.Application/DependencyInjection.cs
using CareRoute.Application.PatientRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace CareRoute.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) // ①
    {
        services.AddScoped<RegisterPatientService>();  // ②
        return services;                               // ③
    }
}