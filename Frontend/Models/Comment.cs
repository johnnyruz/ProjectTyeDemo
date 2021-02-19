using System;

namespace Frontend.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string PhotoId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
