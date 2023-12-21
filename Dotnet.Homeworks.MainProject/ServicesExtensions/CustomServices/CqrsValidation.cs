using Dotnet.Homeworks.Infrastructure.Validation.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.CustomServices;

public static class CqrsValidation
{
    public static IServiceCollection AddCqrsValidation(this IServiceCollection services)
    {
        var featureAssembly = Features.Helpers.AssemblyReference.Assembly;
        
        services.AddMediator(featureAssembly)
            .AddValidatorsFromAssembly(featureAssembly)
            .AddPermissionChecks(featureAssembly);
        
        services.AddTransient(
            typeof(Mediator.IPipelineBehavior<,>), 
            typeof(ValidatorBehavior<,>))
            .AddTransient(
                typeof(Mediator.IPipelineBehavior<,>),
                typeof(PermissionCheckBehavior<,>));

        return services;
    }
}