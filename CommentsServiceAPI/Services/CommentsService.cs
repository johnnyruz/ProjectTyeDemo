using CommentsServiceAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using PhotoServiceAPI.Models;

namespace CommentsServiceAPI.Services
{
    public class CommentsService
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentsService(ICommentsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _comments = database.GetCollection<Comment>(settings.CommentsCollectionName);
        }

        public List<Comment> Get() =>
            _comments.Find(comment => true).ToList();

        public Comment Get(string id) =>
            _comments.Find<Comment>(comment => comment.Id == id).FirstOrDefault();

        public List<Comment> GetCommentsForPhoto(string photoId)
        {
            return _comments.Find(comment => comment.PhotoId == photoId).Sort("{CommentDate: -1}").ToList();
        }

        public Comment Create(Comment commentIn)
        {
            Comment commentOut = new Comment
            {
                CommentDate = DateTime.UtcNow,
                CommentText = commentIn.CommentText,
                PhotoId = commentIn.PhotoId
            };

            _comments.InsertOne(commentOut);
            return commentOut;
        }
        
        public void Update(string id, Comment commentIn) =>
            _comments.ReplaceOne(comment => comment.Id == id, commentIn);

        public void Remove(Comment commentIn) =>
            _comments.DeleteOne(comment => comment.Id == commentIn.Id);

        public void Remove(string id) =>
            _comments.DeleteOne(comment => comment.Id == id);

        public void DeleteAllCommentsForPhoto(string id)
        {
            _comments.DeleteMany(comment => comment.PhotoId == id);
        }
    }
}