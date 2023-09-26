using CardGameProj.Scripts;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

public static class CardDB
{
    public struct CardData
    {
        public int Id;
        public CardTypes Type;
        public CardRanges Range;
        public int Strength;
        public CardNations Nation;
        public CardRanks Rank;
        public string Text;
        public string Category;
        public bool Synergy;        
    }

    private static ReadOnlyDictionary<int, CardData> cards;

    public static ReadOnlyDictionary<int, CardData> GetAllCards
    {
        get { return cards; }
    }

    public static CardData GetCardInfo(int cardId)
    {
        return cards[cardId];
    }

    public static List<CardNations> Nations { get; private set; }

    public static void UpdateCardDataBase()
    {
        if (cards is null)
        {
            string cardDataPath = "Cards.json";
            string jsonData = File.ReadAllText(cardDataPath);

            if (jsonData is null)
            {
                throw new Exception("Data reading failed");
            }

            cards = JsonConvert.DeserializeObject<ReadOnlyDictionary<int, CardData>>(jsonData);

            Nations = Enum.GetValues(typeof(CardNations)).Cast<CardNations>().ToList();
        }
    }
}
