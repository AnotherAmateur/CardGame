using CardGameProj.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    public static string GetCardTexturePath(int cardId)
    {
        var card = cards[cardId];
        return "res://Assets/Cards/" + card.Nation + "/" + card.Type + "/" + cardId + ".png";
    }

    public static string GetFlippedCardTexturePath(CardNations cardNation)
    {
        return "res://Assets/FlippedCards/" + cardNation.ToString() + ".png";
    }

    public static List<CardNations> Nations { get; private set; }

    public static void UpdateCardDataBase()
    {
        if (cards is null)
        {
            string cardDataPath = "res://Data/CardData/Cards.json";
            string jsonData = Godot.FileAccess.Open(cardDataPath, Godot.FileAccess.ModeFlags.Read).GetAsText();

            if (jsonData is null)
            {
                throw new Exception("Data reading failed");
            }

            cards = JsonConvert.DeserializeObject<ReadOnlyDictionary<int, CardData>>(jsonData);

            Nations = Enum.GetValues(typeof(CardNations)).Cast<CardNations>().ToList();

            //Dictionary<int, CardData> CARDS = new();
            //foreach (var card in cards)
            //{
            //    CARDS.Add(card.Key, new(card.Key, card.Value.Type, card.Value.Strength, card.Value.Nation, card.Value.Text));
            //}

            //string fileName = "D:\\Uni\\Курсовая 3 курс\\CardGameGitHub\\CardGame\\GodotProj\\Data\\CardData\\CardsNew.json";
            //string json = JsonConvert.SerializeObject(CARDS, Formatting.Indented);
            //File.WriteAllText(fileName, json);
        }
    }
}