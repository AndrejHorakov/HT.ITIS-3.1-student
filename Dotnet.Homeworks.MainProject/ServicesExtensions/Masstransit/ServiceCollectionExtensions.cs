using Dotnet.Homeworks.MainProject.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitConfiguration = new RabbitMqConfig
        {
            Hostname = configuration["RabbitMqConfig:Hostname"]!,
            Password = configuration["RabbitMqConfig:Password"]!,
            Username = configuration["RabbitMqConfig:Username"]!
        };
        Console.WriteLine(rabbitConfiguration.Hostname);
        Console.WriteLine(rabbitConfiguration.Username);
        Console.WriteLine(rabbitConfiguration.Password);
        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitConfiguration.Hostname, "/",rc =>
                {
                    rc.Username(rabbitConfiguration.Username);
                    rc.Password(rabbitConfiguration.Password);
                });
                config.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}