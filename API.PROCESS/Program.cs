using COMMON.CONTRACTS.Job;
using MassTransit;
using Microsoft.AspNetCore.Components.Routing;
using static MassTransit.Logging.DiagnosticHeaders.Messaging;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.RenderTree;
using COMMON.REPO.Abstraction;
using API.PROCESS.Entities;
using COMMON.REPO.Implementation;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "API.PROCESS", Version = "v1" });
});
builder.Services.AddMassTransit(
    mt =>
    {
        mt.SetKebabCaseEndpointNameFormatter();
        mt.UsingRabbitMq((ctx, cfg) =>
        {

            cfg.Host(builder.Configuration.GetValue<string>("RABBITMQ_HOST"), "/", h =>
            {
                h.Username(builder.Configuration.GetValue<string>("RABBITMQ_USER"));
                h.Password(builder.Configuration.GetValue<string>("RABBITMQ_PSWD"));
            });

            cfg.ConfigureEndpoints(ctx);

        });
    });
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetValue<string>("MONGO_CON");
    return new MongoClient(connectionString);
});

// Register IMongoDatabase in the DI container
builder.Services.AddScoped(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetValue<string>("MONGO_DB");
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<IGenericRepository<JobEnity>>(x => new MongoRepository<JobEnity>(x.GetService<IMongoDatabase>(),
    "Jobs", x.GetRequiredService<ILogger<MongoRepository<JobEnity>>>()));
builder.Services.AddScoped<IGenericRepository<ProcessEntity>>(x => new MongoRepository<ProcessEntity>(x.GetService<IMongoDatabase>(),
    "Processes", x.GetRequiredService<ILogger<MongoRepository<ProcessEntity>>>()));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Process and Jobs API V1");
});


app.UseAuthorization();

app.MapControllers();

app.Run();
