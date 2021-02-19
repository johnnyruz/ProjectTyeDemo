using Frontend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend.HttpClients
{
    public class PhotoClient
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient client;

        public PhotoClient(HttpClient client)
        {
            this.client = client;
        }

        public string GetBaseUrl()
        {
            return this.client.BaseAddress.AbsoluteUri;
        }

        public async Task<Photo[]> GetPhotosAsync()
        {
            var responseMessage = await this.client.GetAsync("/api/photo");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Photo[]>(stream, options);
        }

        public async Task<Photo> GetPhotoAsync(string id)
        {
            var responseMessage = await this.client.GetAsync($"/api/photo/{id}");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Photo>(stream, options);
        }

        public async Task<string> PostPhotoContent(IFormFile photo)
        {
            var fileName = ContentDispositionHeaderValue.Parse(photo.ContentDisposition).FileName.Trim('"');

            try
            {

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(photo.OpenReadStream())
                    {
                        Headers =
                    {
                        ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "photoData", FileName = fileName },
                        ContentLength = photo.Length,
                        ContentType = new MediaTypeHeaderValue(photo.ContentType)
                    }
                    });

                    var response = await this.client.PostAsync("/api/photo", content);
                    var stream = await response.Content.ReadAsStreamAsync();
                    var photoResult = await JsonSerializer.DeserializeAsync<Photo>(stream, options);
                    return photoResult.Id;
                }
            }
            catch
            {
                return "Error Uploading Photo!";
            }
        }
    }
}
