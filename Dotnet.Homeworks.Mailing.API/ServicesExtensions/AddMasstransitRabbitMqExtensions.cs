using Dotnet.Homeworks.Mailing.API.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.Mailing.API.ServicesExtensions;

public static class AddMasstransitRabbitMqExtensions
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
        services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitConfiguration.Hostname, rc =>
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