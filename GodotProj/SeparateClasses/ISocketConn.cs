using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameProj.SeparateClasses
{
	public interface ISocketConn
	{
		void OnHandleError(string exMessage);
		void OnReceiveMessage(string action, string masterId, string message);
	}
}
