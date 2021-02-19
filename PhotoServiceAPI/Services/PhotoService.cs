using PhotoServiceAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace PhotoServiceAPI.Services
{
    public class PhotoService
    {
        private readonly IMongoCollection<Photo> _photos;

        public PhotoService(IPhotosDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _photos = database.GetCollection<Photo>(settings.PhotosCollectionName);
        }

        public bool CheckDbConnection()
        {
            try
            {
                return _photos.CountDocuments("{}") > -1;
            }
            catch
            {
                return false;
            }
        }

        public List<Photo> Get() =>
            _photos.Find(photo => true).Sort("{UploadDate: -1}").ToList();

        public Photo Get(string id) =>
            _photos.Find<Photo>(photo => photo.Id == id).FirstOrDefault();

        public Photo Create(IFormFile photoData)
        {
            IImageFormat format;
            Photo photo;
            
            using (var readStream = photoData.OpenReadStream())
            {
                MemoryStream temp1 = new MemoryStream(4096);

                using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(readStream)) //open the file and detect the file type and decode it
                {
                    image.SaveAsJpeg(temp1);
                    temp1.Seek(0, SeekOrigin.Begin);
                    Image<Rgba32> jpegImage = (Image<Rgba32>)Image.Load(temp1, out format);

                    photo = new Photo
                    {
                        UploadDate = DateTime.UtcNow,
                        OriginalImageData = jpegImage.ToBase64String(format)
                    };

                    jpegImage.Mutate(ctx => ctx.Resize(128, 128));

                    photo.ThumbnailImageData = jpegImage.ToBase64String(format);                    
                }

                _photos.InsertOne(photo);

            }
            return photo;
        }


        // GRID FS Version
        /*
        public Photo Create(IFormFile photoData)
        {
            IImageFormat format;
            var bucket = new GridFSBucket(_photos.Database);
            Photo photo;
            
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            using (var readStream = photoData.OpenReadStream())
            {
                MemoryStream temp = new MemoryStream(4096);

                using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(readStream, out format)) //open the file and detect the file type and decode it
                {
                    image.SaveAsJpeg(temp);
                    temp.Seek(0, SeekOrigin.Begin);
                    var objId = bucket.UploadFromStream("/original/" + fileName, temp);
                    photo = new Photo
                    {
                        OriginalGridFsId = objId.ToString(),
                        UploadDate = DateTime.UtcNow,
                        OriginalImageData = image.ToBase64String(format)
                    };
                }

                _photos.InsertOne(photo);

            }
            return photo;
        }
        */

        public void Update(string id, Photo photoIn) =>
            _photos.ReplaceOne(photo => photo.Id == id, photoIn);

        public void Remove(Photo photoIn) =>
            _photos.DeleteOne(photo => photo.Id == photoIn.Id);

        public void Remove(string id) =>
            _photos.DeleteOne(photo => photo.Id == id);
    }
}