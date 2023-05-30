using System;
using System.Collections.Generic;

namespace CardGameWebApi.DAL.Entities
{
    public partial class User
    {
        public User()
        {
            GameSessionFirstPlayers = new HashSet<GameSession>();
            GameSessionSecondPlayers = new HashSet<GameSession>();
            GameSessionWinners = new HashSet<GameSession>();
        }

        public int UserId { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Rating { get; set; }

        public virtual Lobby Lobby { get; set; } = null!;
        public virtual ICollection<GameSession> GameSessionFirstPlayers { get; set; }
        public virtual ICollection<GameSession> GameSessionSecondPlayers { get; set; }
        public virtual ICollection<GameSession> GameSessionWinners { get; set; }
    }
}
