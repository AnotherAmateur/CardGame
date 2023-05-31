using Godot;

public partial class Antagonist : Player
{
	public static Antagonist Instantiate { get; protected set; }

	public Antagonist(int leaderCard, Control cardRowsContainer, Control leaderCardContainer, Control discardPileContainer, 
		Label totalCount, Control rowsCountContainer) : 
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount, rowsCountContainer)
	{
		Instantiate = this;
	}

	public void PutCardOnBoard(int cardId)
	{
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

