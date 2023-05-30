using CardGameWebApi.BAL;
using CardGameWebApi.DAL.Entities;
using CardGameWebApi.PL.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;

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

		public async Task SendMessage(ActionTypes action, string masterId, string message)
		{
			await Clients.OthersInGroup(masterId).SendAsync("OnGet", action, masterId, message);
		}

		public async Task JoinGroup(string masterId)
		{
			if (groupMasterSlave.TryAdd(masterId, null) || groupMasterSlave[masterId] is null)
			{
				Groups.AddToGroupAsync(Context.ConnectionId, masterId);
				groupMasterSlave[masterId] = (Context.ConnectionId, null);

				dataManager.Lobbies.AddLobby(new Lobby { Master = int.Parse(masterId) });
			}
			else if (groupMasterSlave[masterId].Value.Item2 is null)
			{
				Groups.AddToGroupAsync(Context.ConnectionId, masterId);
				groupMasterSlave[masterId] = (groupMasterSlave[masterId].Value.Item1, Context.ConnectionId);
			}
			else
			{
				Clients.Caller.SendAsync("OnHandleError", "Lobby removed");
			}
		}

		public void ClearGroup(string groupName, int connectionId)
		{
			Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}


		public override async Task OnDisconnectedAsync(Exception exception)
		{
			KeyValuePair<string, (string, string?)?> group = new KeyValuePair<string, (string, string?)?>();

			foreach (var item in groupMasterSlave)
			{
				if (item.Value.Value.Item1 != null && item.Value.Value.Item1.Equals(Context.ConnectionId))
				{
					group = item;
					if (group.Value.Value.Item2 != null)
					{
						Groups.RemoveFromGroupAsync(group.Value.Value.Item2, group.Key);
					}

					break;
				}
				else if (item.Value.Value.Item2 != null && item.Value.Value.Item2.Equals(Context.ConnectionId))
				{
					group = item;
					if (group.Value.Value.Item1 != null)
					{
						Groups.RemoveFromGroupAsync(group.Value.Value.Item1, group.Key);
					}

					break;
				}
			}

			await Clients.Group(group.Key).SendAsync("OnHandleError", "Lobby removed");
			Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Key);

			groupMasterSlave[group.Key] = null;

			var lobby = dataManager.Lobbies.GetLobbyById(int.Parse(group.Key));
			if (lobby != null)
			{
				dataManager.Lobbies.RemoveLobby(lobby);
			}

			// Вызываем базовую реализацию метода OnDisconnectedAsync
			base.OnDisconnectedAsync(exception);
		}
	}
}