using MassTransit;
using MassTransit.SignalR;
using ONBUS.SINGALR.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    var rabbit = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PSWD");
    // Add this for each Hub you have
    x.AddSignalRHub<JobStatusChangedHub>(cfg =>
    {

        /*Configure hub lifetime manager*/

    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbit, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPassword);
        });

        // register consumer' and hub' endpoints
        cfg.ConfigureEndpoints(context);
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
