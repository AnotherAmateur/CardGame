using CardGameProj.Scripts;
using Godot;

public partial class Antagonist : Player
{
	public static Antagonist Instance { get; protected set; }

	public Antagonist(int leaderCard, Control cardRowsContainer, Control leaderCardContainer,
		Control discardPileContainer, Label totalCount, Control rowsCountContainer,
		HBoxContainer roundVBoxContainer) : base(leaderCard, cardRowsContainer, leaderCardContainer, 
			discardPileContainer, totalCount, rowsCountContainer, roundVBoxContainer)
	{
		Instance = this;		
	}

	public void PutCardOnBoard(int cardId)
	{
		var cardInfo = CardDataBase.GetCardInfo(cardId);		
		if (cardInfo.type == CardTypes.Special)
		{
			PutSpecialCard(cardInfo);
		}
		else
		{
			OnBoard.Add(cardInfo.id);
			UpdateBoard();
		}
	}
}

