using MessageContracts;
using CommentsServiceAPI.Models;
using CommentsServiceAPI.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommentsServiceAPI.Controllers
{
    [ApiController]
    [Route("/api/comments")]
    public class CommentServiceController : ControllerBase
    {
        private readonly CommentsService _commentsService;
        private readonly ILogger<CommentServiceController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CommentServiceController(ILogger<CommentServiceController> logger, CommentsService commentsService, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _commentsService = commentsService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public ActionResult<List<Comment>> Get() =>
            _commentsService.Get();

        [HttpGet("{id:length(24)}", Name = "GetComment")]
        public ActionResult<Comment> Get(string id)
        {
            var comment = _commentsService.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpGet("/api/comments/photo/{id:length(24)}", Name = "GetCommentsForPhoto")]
        public ActionResult<List<Comment>> GetCommentsForPhoto(string id)
        {
            var comments = _commentsService.GetCommentsForPhoto(id);
            return comments;
        }


        [HttpPost]
        public ActionResult<Comment> Create(Comment commentIn)
        {
            var comment = _commentsService.Create(commentIn);
            _publishEndpoint.Publish(new CommentAdded()
            {
                PhotoId = comment.PhotoId,
                CommentText = comment.CommentText,
                CommentDate = comment.CommentDate,
                CommentId = comment.Id
            });
            return CreatedAtRoute("GetComment", new { id = comment.Id.ToString() }, comment);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Comment commentIn)
        {
            var comment = _commentsService.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            _commentsService.Update(id, commentIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var comment = _commentsService.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            _commentsService.Remove(comment.Id);

            return NoContent();
        }
    }
}
