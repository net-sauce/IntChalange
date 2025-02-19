using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;


namespace ONBUS.STATUSSERVICE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var rabbit = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                    var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                    var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PSWD");
                    var mongo_con = Environment.GetEnvironmentVariable("MONGO_CON");
                    var mongo_db = Environment.GetEnvironmentVariable("MONGO_DB");

                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        // By default, sagas are in-memory, but should be changed to a durable
                        // saga repository.
                        x.SetInMemorySagaRepositoryProvider();

                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);
                        x.AddSagaStateMachine<JonStateMachine, JobEntity>()
                          .MongoDbRepository(r =>
                          {
                              r.Connection = mongo_con;
                              r.DatabaseName = mongo_db;
                              r.CollectionName = "Jobs";
                          });
                        x.AddSagas(entryAssembly);
                        x.AddActivities(entryAssembly);

                      
                        x.UsingRabbitMq((ctx, cfg) =>
                        {

                            cfg.Host(rabbit, "/", h =>
                            {
                                h.Username(rabbitUser);
                                h.Password(rabbitPassword);
                            });

                            cfg.ConfigureEndpoints(ctx);

                        });
                    });
                });
    }
}
