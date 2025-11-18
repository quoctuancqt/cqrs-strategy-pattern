using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Application.Handlers;
using RecruitmentStrategy.Application.Strategies;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application;

/// <summary>
/// Extension methods for registering application services with Dependency Injection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Register all application services (MediatR handlers and strategies)
    /// </summary>
    public static IServiceCollection AddRecruitmentApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Register all strategy implementations
        services.AddScoped<IRecruitmentSpecificRulesStrategy, ProcessAStrategy>();
        services.AddScoped<IRecruitmentSpecificRulesStrategy, ProcessBStrategy>();
        
        // Register the generic handler for base command
        services.AddScoped<IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>, RecruitmentCommandHandler>();
        
        // Also register handlers for derived commands that delegate to the base handler
        services.AddScoped<IRequestHandler<RecruitmentProcessACommand, RecruitmentResult>>(sp => 
            sp.GetRequiredService<IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>>());
        services.AddScoped<IRequestHandler<RecruitmentProcessBCommand, RecruitmentResult>>(sp => 
            sp.GetRequiredService<IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>>());

        return services;
    }
}


