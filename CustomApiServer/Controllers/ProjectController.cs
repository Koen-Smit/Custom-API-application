using CustomApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace CustomApiServer.Controllers
{
    internal class ProjectController
    {
        [Route("projects")]
        public static string Index(HttpListenerRequest request, HttpListenerResponse response)
        {
            using var db = new AppDbContext();

            // Haal alle projecten op, inclusief bijbehorende gebruikers.
            var projects = db.Projects.Include(p => p.Users).ToList();

            // Bouw een antwoord op waar we alle data via DTO's terugsturen. Zo bepalen
            // we bewust welke data we terugsturen (en lekken we niet per ongeluk gevoelige
            // informatie)
            var dto = new List<ProjectDto>();

            foreach (var project in projects)
            {
                var usersDto = new List<UserDto>();

                foreach (var user in project.Users)
                {
                    usersDto.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username
                    });
                }

                dto.Add(new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Users = usersDto,
                });
            }

            var json = JsonSerializer.Serialize(dto);

            // Stuur de JSON als antwoord
            response.ContentType = "application/json";
            return json;
        }
    }
}
