using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontendBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            ApiBaseAddress apiBase;

            if (builder.HostEnvironment.IsDevelopment())
            {
                apiBase = new ApiBaseAddress
                {
                    ApiBaseUrl = new Uri(builder.HostEnvironment.BaseAddress).GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port,
                               UriFormat.UriEscaped).TrimEnd('/').ToString() + ":8080",
                    SignalRUrl = "http://localhost:55555/phototyehub"
                };
            }
            else
            {
                apiBase = new ApiBaseAddress
                {
                    ApiBaseUrl = "http://project-tye.kubernetes",
                    SignalRUrl = "http://project-tye.kubernetes/phototyehub"
                };
            }

            builder.Services.AddScoped(sp => apiBase);

            await builder.Build().RunAsync();
        }
    }
}
