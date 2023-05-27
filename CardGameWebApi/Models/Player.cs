using System;
using System.Collections.Generic;

namespace CardGameWebApi.Models
{
    public partial class Player
    {
        public Player()
        {
            GameSessionFirstPlayers = new HashSet<GameSession>();
            GameSessionSecondPlayers = new HashSet<GameSession>();
            RoomFirstPlayers = new HashSet<Room>();
            RoomSecondPlayers = new HashSet<Room>();
            SessionEvents = new HashSet<SessionEvent>();
        }

        public int PlayerId { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<GameSession> GameSessionFirstPlayers { get; set; }
        public virtual ICollection<GameSession> GameSessionSecondPlayers { get; set; }
        public virtual ICollection<Room> RoomFirstPlayers { get; set; }
        public virtual ICollection<Room> RoomSecondPlayers { get; set; }
        public virtual ICollection<SessionEvent> SessionEvents { get; set; }
    }
}
