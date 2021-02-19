using MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NotificationServiceAPI.Messaging
{
    public class PhotoDeletedEventConsumer : IConsumer<PhotoDeleted>
    {
        ILogger<PhotoDeletedEventConsumer> _logger;

        public PhotoDeletedEventConsumer(ILogger<PhotoDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PhotoDeleted> context)
        {
            _logger.LogInformation($"SignalR Photo Deleted: {context.Message.PhotoId}");
        }
    }
}
