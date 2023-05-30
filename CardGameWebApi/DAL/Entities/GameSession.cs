using System;
using System.Collections.Generic;

namespace CardGameWebApi.DAL.Entities
{
    public partial class GameSession
    {
        public int SessionId { get; set; }
        public int FirstPlayerId { get; set; }
        public int SecondPlayerId { get; set; }
        public int? WinnerId { get; set; }

        public virtual User FirstPlayer { get; set; } = null!;
        public virtual User SecondPlayer { get; set; } = null!;
        public virtual User? Winner { get; set; }
    }
}
