using System;
using System.Collections.Generic;

namespace CardGameWebApi.Models
{
    public partial class SessionEvent
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public string EventDescrptn { get; set; } = null!;
        public int? CardId { get; set; }

        public virtual Player Player { get; set; } = null!;
    }
}
