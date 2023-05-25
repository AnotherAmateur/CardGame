using Godot;

public class Antagonist : Player
{
	public static Antagonist Instance { get; protected set; }


	public Antagonist(int leaderCard, Control cardRowsContainer, Control leaderCardContainer, Control discardPileContainer, 
		Label totalCount, Control rowsCountContainer) : 
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount, rowsCountContainer)
	{
		Instance = this;
		HTTPRequastInit();
	}


	private void HTTPRequastInit()
	{
		var t = ServerImitation.HTTPRequestInit();
		//SetLeaderCard(t["leaderCard"]);
	}


	public void PutCardOnBoard(int cardId)
	{
		ServerImitation.HTTPRequestGetMove();

		OnBoard.Add(cardId);
		UpdateBoard();
	}

	protected override void UpdateDiscardPileFlippedCard()
	{
		
	}

	protected override void UpdateDeckSize()
	{
	
	}
}

