using System;
using System.Collections.Generic;

namespace CardGameWebApi.DAL.Entities
{
    public partial class Lobby
    {
        public int Master { get; set; }

        public virtual User MasterNavigation { get; set; } = null!;
    }
}
