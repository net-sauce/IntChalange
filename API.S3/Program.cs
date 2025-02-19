using COMMON.AWS.Abstraction;
using COMMON.AWS.Interfance;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "API.S3", Version = "v1" });
});
builder.Services.AddSingleton<IS3Service>(x =>
{
    var bucket = builder.Configuration.GetValue<string>("BUCKET_NAME");
    var endpoint = builder.Configuration.GetValue<string>("MINIO_ENDPOINT");
    var accesskey = builder.Configuration.GetValue<string>("MINIO_ACCESS_KEY");
    var secreet = builder.Configuration.GetValue<string>("MINIO_SECRET_KEY");

    return new MinioS3Service(bucket, endpoint, accesskey, secreet, x.GetService<ILogger<MinioS3Service>>());
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "S3 API V1");
});



app.UseAuthorization();

app.MapControllers();

app.Run();
