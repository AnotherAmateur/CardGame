using CardGameWebApi.EfCore;
using Microsoft.AspNetCore.Mvc;
using CardGameWebApi;
using CardGameWebApi.Models;

namespace Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GameController : ControllerBase
	{
		private readonly CardGameDbContext _context;
		public GameController(CardGameDbContext _context)
		{
			this._context = _context;
		}


		[HttpPost("action")]
		public async Task<IActionResult> Post([FromBody] ActionModel actionModel)
		{
			//_context.Games.Add()
			return Ok();
		}


		[HttpPost("registration")]
		public async Task<IActionResult> Registration([FromBody] User modelUser)
		{
			if (modelUser.Login != null || modelUser.Password != null)
			{
				if (_context.Players.FirstOrDefault(x => x.Login == modelUser.Login) is null)
				{
					var user = _context.Players.Add(new Player() { Login = modelUser.Login, Password = modelUser.Password });

					try
					{
						_context.SaveChanges();
					}
					catch (Exception ex)
					{
						return Unauthorized(ex.Message);
					}

					return Ok(user.Entity.PlayerId);
				}
			}

			return Unauthorized();
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] User modelUser)
		{
			if (modelUser.Login != null || modelUser.Password != null)
			{
				var user = _context.Players.FirstOrDefault(x => x.Login == modelUser.Login);

				if (user != null && modelUser.Password == user.Password)
				{
					return Ok(user.PlayerId);
				}
			}

			return Unauthorized();
		}
	}
}
