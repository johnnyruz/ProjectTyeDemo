using System;

namespace MessageContracts
{
    public class CommentAdded
    {
       public string PhotoId { get; set; }
       public string CommentText { get; set; }
       public DateTime CommentDate { get; set; }
       public string CommentId { get; set; }
    }
}
