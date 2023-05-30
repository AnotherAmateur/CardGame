using CardGameWebApi.DAL.Entities;

namespace CardGameWebApi.BAL.Interfaces
{
	public interface IUserRep
	{
		IEnumerable<User> GetAllUsers();
		User GetUserById(int userId);
		public User? GetUserByLogin(string login);
		int AddUser(User user);
		void RemoveUser(User user);
		public void UpdateUser(User user);
	}
}
