using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace PhotoServiceAPI.Models
{
    public class Photo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UploadDate { get; set; }
        public string ThumbnailImageData { get; set; }
        public string ProcessedImageData { get; set; }
        public bool IsProcessed { get; set; }

        [JsonIgnore]
        public string OriginalImageData { get; set; }
        
    }
}

