using MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationServiceAPI.Hubs;

namespace NotificationServiceAPI.Messaging
{
    public class ProcessingCompleteEventConsumer : IConsumer<ProcessingComplete>
    {
        ILogger<ProcessingCompleteEventConsumer> _logger;
        IHubContext<PhotoTyeHub> _hubContext;

        public ProcessingCompleteEventConsumer(ILogger<ProcessingCompleteEventConsumer> logger, IHubContext<PhotoTyeHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ProcessingComplete> context)
        {
            _logger.LogInformation($"SignalR Processing Complete: {context.Message.PhotoId}");
            await _hubContext.Clients.Group(context.Message.PhotoId).SendAsync("ProcessingComplete");
        }
    }
}
