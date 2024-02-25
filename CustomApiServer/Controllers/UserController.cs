using CustomApiServer.Data;
using System.Net;
using System.Text.Json;

namespace CustomApiServer.Controllers
{
    internal class UserController
    {
        [Route("user", "{userId}")]
        public static string? Show(HttpListenerRequest request, HttpListenerResponse response, int userId)
        {
            using var db = new AppDbContext();

            var user = db.Users.Find(userId);

            if (user == null)
            {
                // 404
                return null;
            }

            // Zet de user om naar JSON met System.Text.Json
            // Om niet de hele user te sturen (inclusief password hash), gebruiken we een
            // data transfer object (DTO) om alleen de gegevens te sturen die we willen.
            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            var json = JsonSerializer.Serialize(dto);

            response.ContentType = "application/json";
            return json;
        }
    }
}
