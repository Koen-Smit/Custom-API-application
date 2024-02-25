using CustomApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace CustomApiServer
{
    internal class Program
    {
        public const string SERVER_BASE_URL = "http://localhost:8080/";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");

            RoutesRepository.DiscoverRoutes();

            InitializeDatabase();

            StartHttpListener(new[]
            {
                SERVER_BASE_URL
            });

            Console.WriteLine("Server stopped.");
        }

        private static void InitializeDatabase()
        {
            using var db = new AppDbContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        private static void StartHttpListener(string[] prefixes)
        {
            using var listener = new HttpListener();

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
                Console.WriteLine($"Listening on {prefix}...");
            }

            listener.Start();
            var shouldListen = true;

            do
            {
                // De GetContext methode blokkeert terwijl op een verzoek van een Client
                // wordt gewacht.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                RoutesRepository.ExecuteRoute(request, response);
            } while (shouldListen);

            listener.Stop();
        }
    }
}