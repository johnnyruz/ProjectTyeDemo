using MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using NotificationServiceAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NotificationServiceAPI.Messaging
{
    public class PhotoAddedEventConsumer : IConsumer<PhotoAdded>
    {
        ILogger<PhotoAddedEventConsumer> _logger;
        IHubContext<PhotoTyeHub> _hubContext;

        public PhotoAddedEventConsumer(ILogger<PhotoAddedEventConsumer> logger, IHubContext<PhotoTyeHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<PhotoAdded> context)
        {
            _logger.LogInformation($"SignalR Photo Added: {context.Message.PhotoId}");
            await _hubContext.Clients.All.SendAsync("NewPhotoAdded");
        }
    }
}
