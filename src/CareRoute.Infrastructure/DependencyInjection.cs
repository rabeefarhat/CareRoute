// src/CareRoute.Infrastructure/DependencyInjection.cs
using CareRoute.Application.PatientRegistry;
using CareRoute.Infrastructure.PatientRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace CareRoute.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPatientStore, InMemoryPatientStore>(); // ①
        return services;
    }
}