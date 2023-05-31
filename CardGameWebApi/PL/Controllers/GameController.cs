using Microsoft.AspNetCore.Mvc;
using CardGameWebApi.PL.Models;
using CardGameWebApi.BAL;
using System.Diagnostics.Metrics;

namespace CardGameWebApi.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GameController : ControllerBase
	{
		private readonly DataManager dataManager;

		public GameController(DataManager dataManager)
		{
			this.dataManager = dataManager;
		}

		[HttpGet("getlobbies")]
		public IActionResult GetLobbyList()
		{
			List<string> lobbies = new List<string>();
			foreach (var item in dataManager.Lobbies.GetAllLobbies())
			{
				var lobby = new LobbyModel();
				var master = dataManager.Users.GetUserById(item.Master);

				lobby.MasterName = master.Login;
				lobby.MasterRating = master.Rating;
				lobby.MasterId = master.UserId;

				lobbies.Add(String.Join(";", lobby.MasterId, lobby.MasterRating, lobby.MasterName));
			}

			return Ok(lobbies);
		}

		[HttpPost("registration")]
		public IActionResult Registration([FromBody] UnAuthorizedUserModel modelUser)
		{
			if (modelUser.Login != null && modelUser.Password != null)
			{
				if (dataManager.Users.GetUserByLogin(modelUser.Login) is null)
				{
					int newUserId = dataManager.Users.AddUser(new() { Login = modelUser.Login, Password = modelUser.Password });

					return Ok(newUserId);
				}
			}

			return Unauthorized();
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] UnAuthorizedUserModel modelUser)
		{
			if (modelUser.Login != null || modelUser.Password != null)
			{
				var user = dataManager.Users.GetUserByLogin(modelUser.Login) ?? null;

				if (user != null && user.Password == modelUser.Password)
				{
					return Ok(user.UserId);
				}
			}

			return Unauthorized();
		}
	}
}
