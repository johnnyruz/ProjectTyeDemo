using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Frontend.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class UploadPhotoModel : PageModel
    {
        [BindProperty]
        public IFormFile Upload { get; set; }
        public string PhotoUploadUrl { get; set; }
        public string ErrorResult { get; set; }

        public void OnGet([FromServices] PhotoClient photoClient)
        {
            PhotoUploadUrl = photoClient.GetBaseUrl() + "api/photo";
        }

        public async Task<ActionResult> OnPostAsync([FromServices] PhotoClient photoClient)
        {
            var uploadResult = await photoClient.PostPhotoContent(Upload);
            if (uploadResult.Length == 24)
            {
                //redirect to View Photo
                return RedirectToPage("/ViewPhoto", new { id = uploadResult });
            }
            else
            {
                ErrorResult = uploadResult;
                return new OkResult();
            }
        }

    }
}
