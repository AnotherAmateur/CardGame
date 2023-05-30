using CardGameWebApi.BAL.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CardGameWebApi.BAL
{
	public class DataManager
	{
		private readonly IUserRep userRep;
		private readonly IGameSessionsRep gameSessionsRep;
		private readonly ILobbyRep lobbyRep;

		public DataManager(IUserRep userRep, IGameSessionsRep gameSessionsRep, ILobbyRep lobbyRep)
		{
			this.userRep = userRep;
			this.gameSessionsRep = gameSessionsRep;
			this.lobbyRep = lobbyRep;
		}

		public IUserRep Users { get { return userRep; } }
		public IGameSessionsRep GameSessions { get {  return gameSessionsRep; } }
		public ILobbyRep Lobbies { get { return lobbyRep; } }
	}
}
