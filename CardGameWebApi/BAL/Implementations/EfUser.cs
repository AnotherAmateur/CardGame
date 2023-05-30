using CardGameWebApi.BAL.Interfaces;
using CardGameWebApi.DAL;
using CardGameWebApi.DAL.Entities;

namespace CardGameWebApi.BAL.Implementations
{
	public class EfUser : IUserRep
	{
		private readonly CardGameDbContext context;

		public EfUser(CardGameDbContext context)
		{
			this.context = context;
		}

		public int AddUser(User user)
		{
			int newUserId = context.Users.Add(user).Entity.UserId;
			context.SaveChanges();
			return newUserId;
		}

		public void UpdateUser(User user)
		{
			context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			context.SaveChanges();
		}

		public IEnumerable<User> GetAllUsers()
		{
			throw new NotImplementedException();
		}

		public User GetUserById(int userId)
		{
			throw new NotImplementedException();
		}

		public User? GetUserByLogin(string login)
		{
			return context.Users.FirstOrDefault(it => it.Login == login);
		}

		public void RemoveUser(User user)
		{
			throw new NotImplementedException();
		}
	}
}
