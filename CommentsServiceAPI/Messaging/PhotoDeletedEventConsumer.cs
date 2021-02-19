using MessageContracts;
using CommentsServiceAPI.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommentsServiceAPI.Messaging
{
    public class PhotoDeletedEventConsumer : IConsumer<PhotoDeleted>
    {
        ILogger<PhotoDeletedEventConsumer> _logger;
        CommentsService _commentsService;

        public PhotoDeletedEventConsumer(ILogger<PhotoDeletedEventConsumer> logger, CommentsService commentsService)
        {
            _logger = logger;
            _commentsService = commentsService;
        }

        public async Task Consume(ConsumeContext<PhotoDeleted> context)
        {
            try
            {
                _commentsService.DeleteAllCommentsForPhoto(context.Message.PhotoId);
            }
            catch { }
        }
    }
}
