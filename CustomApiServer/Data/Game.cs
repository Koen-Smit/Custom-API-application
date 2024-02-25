using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer.Data
{
    internal class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
        public IList<HighScore> HighScores { get; set; }
    }
}
