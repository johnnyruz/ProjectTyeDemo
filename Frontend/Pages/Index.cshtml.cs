using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.HttpClients;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Photo[] PhotoServiceData { get; set; }
        public string SignalRUrl { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet([FromServices] PhotoClient photoClient, [FromServices] ISignalRConnectionSettings signalRSettings)
        {
            PhotoServiceData = await photoClient.GetPhotosAsync();
            SignalRUrl = signalRSettings.ConnectionString;
        }
    }
}
