using CardGameWebApi.DAL.Entities;

namespace CardGameWebApi.BAL.Interfaces
{
	public interface IGameSessionsRep
	{
		IEnumerable<GameSession> GetAllSessions();
		IEnumerable<GameSession> GetSessionsById(int sessionId);
		void AddSession(GameSession session);
		void RemoveSession(GameSession session);
		void UpdateSession(GameSession session);
	}
}
