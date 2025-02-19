using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ConsoleAppWithHost
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "rbuser";
            var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PSWD") ?? "rbpass";

            // Create and run the host
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    // Add MassTransit configuration
                    services.AddMassTransit(config =>
                    {
                        config.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(rabbitHost, "/", h =>
                            {
                                h.Username(rabbitUser);
                                h.Password(rabbitPassword);
                            });
                        });
                    });

                    // Register services
                    services.AddSingleton<MinioService>(); // Minio client service
                    services.AddHostedService<MinioBackgroundService>(); // Background worker for Minio bucket notifications
                })
                .RunConsoleAsync();

            Console.WriteLine("Application is shutting down.");
        }
    }
}