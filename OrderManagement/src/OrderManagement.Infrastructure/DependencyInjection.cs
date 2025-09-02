using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Infrastructure.Data.Repositories;
using OrderManagement.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, b =>
            {
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                b.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }

            // Enable detailed errors in development
            if (configuration.GetValue<bool>("EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }
        });

        // Register DbContext as IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Repository Pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External Services
        services.AddScoped<IEmailService, EmailService>();

        // Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database",
                customTestQuery: async (context, cancellationToken) =>
                {
                    var result = await context.Database.CanConnectAsync(cancellationToken);
                    return result;
                });

        return services;
    }

    public static IServiceCollection AddInfrastructureForTesting(
        this IServiceCollection services,
        string connectionString)
    {
        // In-Memory Database for Testing
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(connectionString);
            options.EnableSensitiveDataLogging();
        });

        // Register DbContext as IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Repository Pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Mock External Services
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}