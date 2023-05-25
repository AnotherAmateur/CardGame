using System.Collections.Generic;

public static class ServerImitation
{
	public static Dictionary<string, string> HTTPRequestInit()
	{
		Dictionary<string, string> response = new();
		//response.Add("leaderCard", CardSelectionMenu.LeaderCard);
		response.Add("deckCount", CardSelectionMenu.SelectedCards.Count.ToString());

		return response;
	}


	public static Dictionary<string, string> HTTPRequestGetMove()
	{
		Dictionary<string, string> response = new();
		//response.Add("cardId", CardSelectionMenu.LeaderCard);
		response.Add("deckCount", CardSelectionMenu.SelectedCards.Count.ToString());

		return response;
	}

}

