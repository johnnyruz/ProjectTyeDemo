using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MessageContracts;
using PhotoServiceAPI.Models;
using PhotoServiceAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoServiceAPI.Controllers
{

    [ApiController]
    [Route("/api/photo")]
    public class PhotoServiceController : ControllerBase
    {

        private readonly PhotoService _photoService;
        private readonly ILogger<PhotoServiceController> _logger;
        readonly IPublishEndpoint _publishEndpoint;

        public PhotoServiceController(ILogger<PhotoServiceController> logger, PhotoService photoService, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _photoService = photoService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public ActionResult<List<Photo>> Get() =>
            _photoService.Get();

        [HttpGet("{id:length(24)}", Name = "GetPhoto")]
        public ActionResult<Photo> Get(string id)
        {
            var photo = _photoService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            return photo;
        }

        
        [HttpPost]
        public ActionResult<Photo> Create(IFormFile photoData)
        {
            var photo = _photoService.Create(photoData);
            _publishEndpoint.Publish(new PhotoAdded()
            {
                PhotoId = photo.Id.ToString()
            });
            return CreatedAtRoute("GetPhoto", new { id = photo.Id.ToString() }, photo);
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Photo photoIn)
        {
            var photo = _photoService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            _photoService.Update(id, photoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            var photo = _photoService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            _photoService.Remove(photo.Id);

            _publishEndpoint.Publish(new PhotoDeleted()
            {
                PhotoId = id
            });

            return NoContent();
        }
    }
}
