using CardGameWebApi.BAL.Interfaces;
using CardGameWebApi.DAL;
using CardGameWebApi.DAL.Entities;
using CardGameWebApi.PL.Models;

namespace CardGameWebApi.BAL.Implementations
{
	public class EfLobby : ILobbyRep
	{
		private readonly CardGameDbContext context;

		public EfLobby(CardGameDbContext context)
		{
			this.context = context;
		}

		public void AddLobby(Lobby lobby)
		{
			context.Lobbies.Add(lobby);
			context.SaveChanges();
		}

		public IEnumerable<Lobby> GetAllLobbies()
		{
			return context.Lobbies.ToList();
		}

		public Lobby? GetLobbyById(int masterId)
		{
			return context.Lobbies.FirstOrDefault(it => it.Master == masterId);
		}

		public void RemoveLobby(Lobby lobby)
		{
			context.Lobbies.Remove(lobby);
			context.SaveChanges();
		}
	}
}
