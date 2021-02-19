using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CommentsServiceAPI.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string PhotoId { get; set; }
        public string CommentText { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CommentDate { get; set; }
    }
}
