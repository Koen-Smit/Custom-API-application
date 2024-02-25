using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Custom_API_Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string ApiBaseUrl = "http://localhost:8080/highscore/";
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void ShowHighScoreDetails(object sender, RoutedEventArgs e)
        {
            string highScoreId = HighScoreIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(highScoreId))
            {
                // Handle empty input
                return;
            }

            string apiUrl = ApiBaseUrl + highScoreId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON response
                        HighScore highScore = JsonSerializer.Deserialize<HighScore>(json);

                        // Display high score details
                        HighScoreDetailsTextBlock.Text = $"ID: {highScore.Id}\n" +
                                                        $"Score: {highScore.Score}\n" +
                                                        $"Game: {highScore.Game}\n";
                    }
                    else
                    {
                        HighScoreDetailsTextBlock.Text = "High score not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                HighScoreDetailsTextBlock.Text = $"Error: {ex.Message}";
            }
        }
    }

    public class HighScore
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public string Game { get; set; }
    }
}
