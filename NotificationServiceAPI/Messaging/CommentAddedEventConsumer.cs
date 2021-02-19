using MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NotificationServiceAPI.Hubs;

namespace NotificationServiceAPI.Messaging
{
    public class CommentAddedEventConsumer : IConsumer<CommentAdded>
    {
        ILogger<CommentAddedEventConsumer> _logger;
        IHubContext<PhotoTyeHub> _hubContext;

        public CommentAddedEventConsumer(ILogger<CommentAddedEventConsumer> logger, IHubContext<PhotoTyeHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<CommentAdded> context)
        {
            _logger.LogInformation($"SignalR Comment Added: {context.Message.PhotoId}");
            await _hubContext.Clients.Group(context.Message.PhotoId).SendAsync("CommentAdded", context.Message.CommentText, context.Message.CommentDate.ToString("G"), context.Message.CommentId);
        }
    }
}
