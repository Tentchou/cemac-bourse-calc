using CemacBourseCalc.Application.Common.Behaviors;
using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CemacBourseCalc.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ExecuteurEnMasse>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
