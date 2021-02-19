using Frontend.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend.HttpClients
{
    public class CommentClient
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient client;

        public CommentClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<string> GetCommentsAsync()
        {
            var responseMessage = await this.client.GetAsync("/api/comments");
            var stream = await responseMessage.Content.ReadAsStringAsync();
            return stream;
        }

        public async Task<Comment[]> GetCommentsForPhotoAsync(string photoId)
        {
            var responseMessage = await this.client.GetAsync($"/api/comments/photo/{photoId}");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Comment[]>(stream, options);
        }

        public async Task<string> PostCommentForPhoto(Comment comment)
        {
            try
            {
                var objAsJson = JsonSerializer.Serialize(comment);
                var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
                var response = await this.client.PostAsync("/api/comments", content);
                var stream = await response.Content.ReadAsStreamAsync();
                var commentResult = await JsonSerializer.DeserializeAsync<Comment>(stream, options);
                return commentResult.Id;
            }catch
            {
                return "Error Posting New Comment";
            }
        }
    }
}
