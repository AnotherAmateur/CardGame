namespace CardGameProj.Scripts
{
	public enum CardTypes { Group1, Group2, Group3, Leader, Special }
	public enum CardNations { AI, Confucius };
	public enum CardRanks { Common, Rare, Legendary };
	public enum ActionTypes
	{
        Pass = -200,
        CardMove,		
		Exit,
		Start,
		Join,
		Ready,
		GameOver,
		Disconnected,
		FirstPlayer,
		DeckSizeUpdated
	}
	public enum CardEvents { LeftCllick, RightClick }
}
