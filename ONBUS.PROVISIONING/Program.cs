using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace ONBUS.PROVISIONING
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", false, true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
                {

                    services.AddMassTransit(x =>
                    {

                        var config = hostContext.Configuration;

                        x.SetKebabCaseEndpointNameFormatter
                        ();

                        // By default, sagas are in-memory, but should be changed to a durable
                        // saga repository.
                       // x.SetInMemorySagaRepositoryProvider();

                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);
                        //x.AddSagaStateMachines(entryAssembly);
                        //x.AddSagas(entryAssembly);
                        //x.AddActivities(entryAssembly);

                        x.UsingRabbitMq((ctx, cfg) =>
                        {

                            cfg.Host(config.GetValue<string>("RABBITMQ_HOST"), "/", h =>
                            {
                                h.Username(config.GetValue<string>("RABBITMQ_USER"));
                                h.Password(config.GetValue<string>("RABBITMQ_HOST"));
                            });

                            cfg.ConfigureEndpoints(ctx);

                        });
                    });
                });
    }
}
