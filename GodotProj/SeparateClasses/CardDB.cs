using CardGameProj.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public CardData(string name, int id, CardTypes type, int strength, CardNations nation, string text, CardRanges range = 0)
        {
            Id = id;
            Type = type;
            Strength = strength;
            Nation = nation;
            Text = text;

            switch (strength)
            {
                case < 10:
                    this.Rank = CardRanks.Common;
                    break;
                case < 20:
                    this.Rank = CardRanks.Rare;
                    break;
                default:
                    this.Rank = CardRanks.Legendary;
                    break;
            }

            switch (type)
            {
                case CardTypes.Group1:
                    Category = "Мудрость и философия";
                    Range = CardRanges.Row1;
                    break;
                case CardTypes.Group2:
                    Category = "Личностное развитие и мотивация";
                    Range = CardRanges.Row2;
                    break;
                case CardTypes.Group3:
                    Category = "Социальная справедливость и этика";
                    Range = CardRanges.Row3;
                    break;
                case CardTypes.Leader:
                    Category = nation + ", мудрец";
                    Range = CardRanges.OutOfRange;
                    strength = 0;
                    break;
                case CardTypes.Special:
                    Category = "Специальная карта";
                    switch (Strength)
                    {
                        case < 0:
                            Range = CardRanges.Row1;
                            break;
                        case > 0:
                            Range = CardRanges.Row2;
                            break;
                        default:
                            Range = CardRanges.OutOfRange;
                            break;
                    }
                    break;
            }
        }
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