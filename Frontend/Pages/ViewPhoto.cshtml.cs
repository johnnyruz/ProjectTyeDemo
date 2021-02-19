using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.HttpClients;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class ViewPhotoModel : PageModel
    {
        public Photo PhotoData { get; set; }
        public Comment[] Comments { get; set; }

        public string SignalRUrl { get; set; }

        public async Task OnGet(string id, [FromServices] PhotoClient photoClient, [FromServices] CommentClient commentClient, [FromServices] ISignalRConnectionSettings signalRSettings)
        {
            PhotoData = await photoClient.GetPhotoAsync(id);
            Comments = await commentClient.GetCommentsForPhotoAsync(id);
            SignalRUrl = signalRSettings.ConnectionString;
        }

        public async Task<ActionResult> OnPostAsync([FromServices] CommentClient commentClient)
        {
            var commentText = Request.Form["comment_text"];
            var photoId = Request.Form["photo_id"];

            await commentClient.PostCommentForPhoto(new Comment() { CommentText = commentText, PhotoId = photoId });

            return RedirectToAction("/ViewPhoto", new { id = photoId });
        }
    }
}
