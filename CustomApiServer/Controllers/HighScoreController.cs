using CustomApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomApiServer.Controllers
{
    internal class HighScoreController
    {
        [Route("highscore", "{highscoreId}")]
        public static string? Show(HttpListenerRequest request, HttpListenerResponse response, int highscoreId)
        {
            using var db = new AppDbContext();

            var highscore = db.HighScores.Find(highscoreId);

            if (highscore == null)
            {
                // 404
                return null;
            }

            // Zet de user om naar JSON met System.Text.Json
            // Om niet de hele user te sturen (inclusief password hash), gebruiken we een
            // data transfer object (DTO) om alleen de gegevens te sturen die we willen.
            var dto = new HighScore
            {
                Id = highscore.Id,
                Score = highscore.Score,
                DateTime = highscore.DateTime,
                UserId = highscore.UserId,
                GameId = highscore.GameId,
            };
            var json = JsonSerializer.Serialize(dto);

            response.ContentType = "application/json";
            return json;
        }

        [Route("highscores")]
        public static string Index(HttpListenerRequest request, HttpListenerResponse response)
        {
            using var db = new AppDbContext();

            // Haal alle projecten op, inclusief bijbehorende gebruikers.
            var highscores = db.HighScores.ToList();

            // Bouw een antwoord op waar we alle data via DTO's terugsturen. Zo bepalen
            // we bewust welke data we terugsturen (en lekken we niet per ongeluk gevoelige
            // informatie)
            var dto = new List<HighScore>();

            foreach (var highscore in highscores)
            {

                dto.Add(new HighScore
                {
                    Id = highscore.Id,
                    Score = highscore.Score,
                    DateTime = highscore.DateTime,
                    UserId = highscore.UserId,
                    GameId = highscore.GameId,
                });
            }

            var json = JsonSerializer.Serialize(dto);

            // Stuur de JSON als antwoord
            response.ContentType = "application/json";
            return json;
        }
    }
}
