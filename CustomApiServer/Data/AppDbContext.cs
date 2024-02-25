using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<HighScore> HighScores { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;database=sd_custom_api_server;user=root;password=;",
                ServerVersion.Parse("8.0.30")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Admin",
                    Email = "aapjeAdmin@aapje.nl",
                    HashedPassword = SecureHasher.Hash("secret"),
                },
                new User
                {
                    Id = 2,
                    Username = "Anne",
                    Email = "aapjeAnne@oeoeaa.nl",
                    HashedPassword = SecureHasher.Hash("secret"),
                }
            );

            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Title = "Broodjeaap",
                    Platform = "Weet ik het, Kies zelf",
                },
                new Game
                {
                    Id = 2,
                    Title = "Broodjegorilla",
                    Platform = "Deze weet ik wel, playstation 4",
                }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    Name = "Highscore",
                    Description = "Games en gebruikers van games",
                }
            );

            modelBuilder.Entity<HighScore>().HasData(
                new HighScore
                {
                    Id = 1,
                    Score = 420,
                    DateTime = DateTime.UtcNow,
                    UserId = 1,
                    GameId = 1,
                },
                new HighScore
                {
                    Id = 2,
                    Score = 500,
                    DateTime = DateTime.UtcNow,
                    UserId = 2,
                    GameId = 2,
                }
            );


            // Many-to-many seed zonder pivot model:
            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithMany(p => p.Users)
                .UsingEntity(
                    p => p.HasData(
                        new { ProjectsId = 1, UsersId = 1 },
                        new { ProjectsId = 1, UsersId = 2 }
                    )
                );

            modelBuilder.Entity<HighScore>()
                .HasOne(hs => hs.User)
                .WithMany(u => u.HighScores)
                .HasForeignKey(hs => hs.UserId);

            modelBuilder.Entity<HighScore>()
                .HasOne(hs => hs.Game)
                .WithMany(g => g.HighScores)
                .HasForeignKey(hs => hs.GameId);
        }
    }
}
