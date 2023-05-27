using System;
using System.Collections.Generic;

namespace CardGameWebApi.Models
{
    public partial class GameSession
    {
        public int SessionId { get; set; }
        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
        public byte Status { get; set; }

        public virtual Player FirstPlayer { get; set; } = null!;
        public virtual Player SecondPlayer { get; set; } = null!;
    }
}
