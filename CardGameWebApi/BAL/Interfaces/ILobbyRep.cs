using CardGameWebApi.DAL.Entities;

namespace CardGameWebApi.BAL.Interfaces
{
	public interface ILobbyRep
	{
		IEnumerable<Lobby> GetAllLobbies();
		Lobby? GetLobbyById(int masterId);
		void AddLobby(Lobby lobby);
		void RemoveLobby(Lobby lobby);
	}
}
