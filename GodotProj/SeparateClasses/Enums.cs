using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameProj.Scripts
{
	public enum CardTypes { Group1, Group2, Group3, Leader, Special }
	public enum CardNations { AI, Confucius };
	public enum CardRanks { Common, Rare, Legendary };
	public enum ActionTypes { CardMove, Pass, Exit, Start, Join, Ready }
	public enum CardEvents { LeftCllick, RightClick }
}
