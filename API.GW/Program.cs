using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json",true,true)
    .AddJsonFile("gw.settings.json",false,true).AddEnvironmentVariables();

builder.Services.AddMvcCore();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
// Limit body to 10 MB (S3 limit)
builder.Services.Configure<FormOptions>(x => x.MultipartBodyLengthLimit = 10 * 1024 * 1024);


var app = builder.Build();

await app.UseSwaggerForOcelotUI().UseOcelot();



app.Run();
