using CardGameProj.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public static class CardDataBase
{
    static Random rand = new Random();

    public struct CardData
    {
        public int id;
        public CardTypes type;
        public int strength;
        public CardNations nation;
        public CardRanks rank;
        public string text;
        public string category;

        public CardData(int id, CardTypes type, int strength, CardNations nation, string text)
        {
            this.id = id;
            this.type = type;
            this.strength = strength;
            //this.strength = rand.Next(1, 31);
            this.nation = nation;
            this.text = text;

            switch (strength)
            {
                case < 10:
                    this.rank = CardRanks.Common;
                    break;
                case < 20:
                    this.rank = CardRanks.Rare;
                    break;
                default:
                    this.rank = CardRanks.Legendary;
                    break;
            }

            switch (type)
            {
                case CardTypes.Group1:
                    category = "Мудрость и философия";
                    break;
                case CardTypes.Group2:
                    category = "Личностное развитие и мотивация";
                    break;
                case CardTypes.Group3:
                    category = "Социальная справедливость и этика";
                    break;
                case CardTypes.Leader:
                    category = nation + ", мудрец";
                    strength = 0;
                    break;
                case CardTypes.Special:
                    category = "Специальная карта";
                    break;
            }
        }
    }

    private static Dictionary<int, CardData> cards;

    public static Dictionary<int, CardData> GetAllCards
    {
        get { return new Dictionary<int, CardData>(cards); }
    }

    public static CardData GetCardInfo(int cardId)
    {
        return cards[cardId];
    }

    public static string GetCardTexturePath(int cardId)
    {
        var card = cards[cardId];
        return "res://Assets/Cards/" + card.nation + "/" + card.type + "/" + cardId + ".png";
    }

    public static string GetFlippedCardTexturePath(CardNations cardNation)
    {
        return "res://Assets/FlippedCards/" + cardNation.ToString() + ".png";
    }

    public static List<CardNations> Nations { get; private set; }

    public static void UpdateCardDataBase()
    {
        string cardDataPath = "res://Data/CardData/Cards.json";
        string jsonData = Godot.FileAccess.Open(cardDataPath, Godot.FileAccess.ModeFlags.Read).GetAsText();

        if (jsonData is null)
        {
            throw new Exception("Data reading failed");
        }

        cards = JsonConvert.DeserializeObject<Dictionary<int, CardData>>(jsonData);

        Nations = Enum.GetValues(typeof(CardNations)).Cast<CardNations>().ToList();
    }
}