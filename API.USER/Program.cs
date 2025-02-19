using API.USER.Entities;
using COMMON.REPO.Abstraction;
using COMMON.REPO.Implementation;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "API.USER", Version = "v1" });
});
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetValue<string>("MONGO_CON");
    return new MongoClient(connectionString);
});
builder.Services.AddScoped(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetValue<string>("MONGO_DB");
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<IGenericRepository<UserEntity>>(x => new MongoRepository<UserEntity>(x.GetService<IMongoDatabase>(),
                                                            "Users", x.GetRequiredService<ILogger<MongoRepository<UserEntity>>>()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>()); ;
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
