using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer.Data
{
    internal class HighScore
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime DateTime { get; set; }

        // foreign keys
        public int UserId { get; set; }
        public User User { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
