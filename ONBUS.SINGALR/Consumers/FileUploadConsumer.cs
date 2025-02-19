using MassTransit.Contracts.JobService;
using MassTransit;
using COMMON.CONTRACTS.Job;
using Microsoft.AspNetCore.SignalR;
using ONBUS.SINGALR.Hub;

namespace ONBUS.SINGALR.Consumers
{
    public class JobCompletedConsumeronsumerDefinition : ConsumerDefinition<JobCompletedConsumer>
    {

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<JobCompletedConsumer> consumerConfigurator)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator e)
            {
                e.AutoDelete = true;
                e.QueueExpiration = TimeSpan.FromSeconds(30);
            }
        }
    }
    public class JobCompletedConsumer : IConsumer<OnJobCompleted>
    {
        private readonly IHubContext<JobStatusChangedHub> _hubContext;

        public JobCompletedConsumer(IHubContext<JobStatusChangedHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<OnJobCompleted> context)
        {
            await _hubContext.Clients.Group(context.Message.ClientID.ToString()).SendAsync("JobCompleted", context.Message);
        }
    }
}
