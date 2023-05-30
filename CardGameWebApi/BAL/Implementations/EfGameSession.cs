using CardGameWebApi.BAL.Interfaces;
using CardGameWebApi.DAL;
using CardGameWebApi.DAL.Entities;

namespace CardGameWebApi.BAL.Implementations
{
	public class EfGameSession : IGameSessionsRep
	{
		private readonly CardGameDbContext context;

		public EfGameSession(CardGameDbContext context)
		{
			this.context = context;
		}

		public void AddSession(GameSession session)
		{
			context.GameSessions.Add(session);
			context.SaveChanges();
		}

		public void UpdateSession(GameSession session)
		{
			context.Entry(session).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			context.SaveChanges();
		}

		public IEnumerable<GameSession> GetAllSessions()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<GameSession> GetSessionsById(int sessionId)
		{
			throw new NotImplementedException();
		}

		public void RemoveSession(GameSession session)
		{
			throw new NotImplementedException();
		}
	}
}
