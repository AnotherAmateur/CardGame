using CardGameWebApi.BAL;
using CardGameWebApi.DAL.Entities;
using CardGameWebApi.PL.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CardGameWebApi.PL.Controllers
{
	public class WebSocketController : Hub
	{
		private static ConcurrentDictionary<string, (string, string?)?> groupMasterSlave = new ConcurrentDictionary<string, (string, string?)?>();
		private DataManager dataManager;

		public WebSocketController(DataManager dataManager) : base()
		{
			this.dataManager = dataManager;
		}

		public async Task SendMessage(string action, string masterId, string message)
		{
			await Clients.OthersInGroup(masterId).SendAsync("send", action, masterId, message);
		}

		public async Task JoinGroup(string masterId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, masterId);

			if (groupMasterSlave.TryAdd(masterId, null) || groupMasterSlave[masterId] is null)
			{
				groupMasterSlave[masterId] = (Context.ConnectionId, null);
				dataManager.Lobbies.AddLobby(new Lobby { Master = int.Parse(masterId) });
			}
			else if (groupMasterSlave[masterId].Value.Item2 is null)
			{
				groupMasterSlave[masterId] = (groupMasterSlave[masterId].Value.Item1, Context.ConnectionId);
				Clients.All.SendAsync("send", ActionTypes.Start.ToString(), masterId.ToString(), "nomessage");

				try
				{
					dataManager.Lobbies.RemoveLobby(dataManager.Lobbies.GetLobbyById(int.Parse(masterId)));
				}
				catch (Exception ex)
				{
					Console.Out.WriteLineAsync("Attempt to delete null lobby. WebSocketClient:JoinGroup");
				}

			}
			else
			{
				Clients.Group(masterId).SendAsync("OnHandleError", "Lobby removed");
			}
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			KeyValuePair<string, (string, string?)?> group = new KeyValuePair<string, (string, string?)?>();

			foreach (var item in groupMasterSlave)
			{
				if (item.Value != null)
				{
					if (item.Value.Value.Item1 != null && item.Value.Value.Item1.Equals(Context.ConnectionId))
					{
						group = item;
						await Clients.Group(group.Key).SendAsync("OnHandleError", "Disconnected");
						Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Key);

						if (group.Value.Value.Item2 != null)
						{
							Groups.RemoveFromGroupAsync(group.Value.Value.Item2, group.Key);
						}

						break;
					}
					else if (item.Value.Value.Item2 != null && item.Value.Value.Item2.Equals(Context.ConnectionId))
					{
						group = item;
						await Clients.Group(group.Key).SendAsync("OnHandleError", "Disconnected");
						Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Key);

						if (group.Value.Value.Item1 != null)
						{
							await Groups.RemoveFromGroupAsync(group.Value.Value.Item1, group.Key);
						}

						break;
					}
				}
			}

			if (group.Key != null)
			{
				groupMasterSlave[group.Key] = null;

				var lobby = dataManager.Lobbies.GetLobbyById(int.Parse(group.Key));
				if (lobby != null)
				{
					dataManager.Lobbies.RemoveLobby(lobby);
				}
			}

			// Вызываем базовую реализацию метода OnDisconnectedAsync
			base.OnDisconnectedAsync(exception);
		}
	}
}