using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameProj.SeparateClasses
{
	public class LobbyModel
	{
		public int MasterId { get; set; }
		public int MasterRating { get; set; }
		public string MasterName { get; set; }


		public LobbyModel(int masterId, int masterRating, string masterName)
		{
			MasterId = masterId;
			MasterRating = masterRating;
			MasterName = masterName;
		}		
	}
}
