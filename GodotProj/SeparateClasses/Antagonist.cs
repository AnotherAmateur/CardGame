using CardGameProj.Scripts;
using Godot;

public partial class Antagonist : Player
{
	public static Antagonist Instance { get; private set; }

	public Antagonist(int leaderCard, Control cardRowsContainer, Control leaderCardContainer,
		Control discardPileContainer, Label totalCount, Control rowsCountContainer,
		HBoxContainer roundVBoxContainer) : base(leaderCard, cardRowsContainer, leaderCardContainer, 
			discardPileContainer, totalCount, rowsCountContainer, roundVBoxContainer)
	{
		Instance = this;		
	}

	public void PutCardOnBoard(int cardId)
	{
		var cardInfo = CardDB.GetCardInfo(cardId);
		DisplaySelectedCard(cardInfo);

        if (cardInfo.Type == CardTypes.Special)
		{
            PutSpecialCard(cardInfo);
		}
		else
		{
			OnBoard.Add(cardInfo.Id);
			UpdateBoard();
		}
	}
}

