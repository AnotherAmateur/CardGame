using CardGameProj.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

public abstract class Player
{
    private Label TotalCountLabel;
    protected Control CardRowsContainer;
    protected Control LeaderCardContainer;
    protected Control DiscardPileContainer;
    protected Control RowsCountContainer;
    private HBoxContainer RoundVBoxContainer;
    private VBoxContainer RowSpCardsContainer;
    private HBoxContainer SpCardsContainer;
    private static int round;
    public const int MaxHandSize = 10;
    protected int DiscardPileFlippedcardId;
    public bool IsPass { get; protected set; }
    public static int GameResult { get; protected set; }
    public SortedDictionary<CardRanges, int> TotalsByRows { get; private set; }
    public List<int> DiscardPile { get; protected set; }
    public List<int> OnBoard { get; protected set; }
    public CardDB.CardData LeaderCard { get; protected set; }   
    public static List<CardDB.CardData> SpOnBoard { get; private set; }
    public int TotalCount
    {
        get { return int.Parse(TotalCountLabel.Text); }
        private set { TotalCountLabel.Text = value.ToString(); }
    }


    public Player(int leaderCard, Control cardRowsContainer, Control leaderCardContainer,
        Control discardPileContainer, Label totalCount, Control rowsCountContainer, HBoxContainer roundVBoxContainer)
    {
        DiscardPile = new();
        OnBoard = new();
        InitTotalsByRows();
        SpOnBoard = new();
        LeaderCard = CardDB.GetCardInfo(leaderCard);
        CardRowsContainer = cardRowsContainer;
        LeaderCardContainer = leaderCardContainer;
        DiscardPileContainer = discardPileContainer;
        TotalCountLabel = totalCount;
        RoundVBoxContainer = roundVBoxContainer;
        round = 1;
        GameResult = 0;
        RowSpCardsContainer = GFieldController.Instance.RowSpCardsContainer;
        SpCardsContainer = GFieldController.Instance.SpCardsContainer;

        RowsCountContainer = rowsCountContainer;

        SetLeaderCard(leaderCard);

        UpdateRowsCount();
        UpdateTotalCount();
    }

    private void UpdateTotalCount()
    {
        int totalCount = 0;
        foreach (Label rowCount in RowsCountContainer.GetChildren())
        {
            totalCount += int.Parse((rowCount.Text == "") ? "0" : rowCount.Text);
        }

        TotalCount = totalCount;
    }

    private void UpdateRowsCount()
    {
        InitTotalsByRows();

        foreach (var card in OnBoard.Select(x => CardDB.GetCardInfo(x)))
        {
            TotalsByRows[card.Range] += card.Strength;
        }

        foreach (var spCard in SpOnBoard)
        {
            TotalsByRows[spCard.Range] = spCard.Strength < 0 ?
                OnBoard.Where(x => CardDB.GetCardInfo(x).Range == spCard.Range).Count():
                throw new NotImplementedException();
        }

        foreach (var range in TotalsByRows)
        {
            RowsCountContainer.GetNode<Label>(range.Key.ToString()).Text = range.Value.ToString();
        }
    }

    public void InitTotalsByRows()
    {
        TotalsByRows = new();
        TotalsByRows.Add(CardRanges.Row1, 0);
        TotalsByRows.Add(CardRanges.Row2, 0);
        TotalsByRows.Add(CardRanges.Row3, 0);
    }

    public void SetLeaderCard(int cardId)
    {
        MinCardScene cardInstance = (MinCardScene)GFieldController.MinCardScene.Instantiate();
        LeaderCardContainer.AddChild(cardInstance);
        cardInstance.SetParams(LeaderCardContainer.Size,
            CardDB.GetCardTexturePath(cardId), LeaderCard);
        cardInstance.Name = cardId.ToString();
    }

    public void ClearBoard()
    {
        foreach (Node row in CardRowsContainer.GetChildren())
        {
            foreach (Node node in row.GetChildren())
            {
                if (node is Label)
                {
                    continue;
                }

                row.RemoveChild(node);
            }
        }
    }

    private void CleanSpCards()
    {
        foreach (Node row in SpCardsContainer.GetChildren())
        {
            foreach (MinCardScene node in row.GetChildren())
            {
                row.RemoveChild(node);
            }
        }

        SpOnBoard = new();
    }

    public void UpdateBoard()
    {
        ClearBoard();

        Vector2 rowRectSize = GFieldController.CardRowSize;
        int extraSpaceBtwnCards = 5;
        Vector2 cardSize = new(rowRectSize.X / MaxHandSize - extraSpaceBtwnCards, rowRectSize.Y);

        Dictionary<CardTypes, List<int>> rangeSortedCards = new();

        foreach (int card in OnBoard)
        {
            var cardinfo = CardDB.GetCardInfo(card);
            if (rangeSortedCards.ContainsKey(cardinfo.Type) is false)
            {
                rangeSortedCards.Add(cardinfo.Type, new());
            }

            rangeSortedCards[cardinfo.Type].Add(card);
        }

        int cardMarginRight = 5;
        foreach (var range in rangeSortedCards)
        {
            Control row = CardRowsContainer.GetNode<Control>("Row" + ((int)range.Key + 1));
            float nextXCardPosition = (rowRectSize.X - cardSize.X * range.Value.Count) / 2;
            foreach (int cardId in range.Value)
            {
                MinCardScene cardInstance = (MinCardScene)GFieldController.MinCardScene.Instantiate();
                row.AddChild(cardInstance);
                cardInstance.Name = cardId.ToString();

                var card = row.GetNode<MinCardScene>(cardId.ToString());
                card.SetParams(cardSize, CardDB.GetCardTexturePath(cardId), CardDB.GetCardInfo(cardId));
                card.Position = new Vector2(nextXCardPosition, 0);
                nextXCardPosition += cardSize.X + cardMarginRight;
            }
        }

        UpdateRowsCount();
        UpdateTotalCount();
    }

    public void DoPass()
    {
        IsPass = true;
        var gameCtrl = GFieldController.Instance;
        var protagonist = Protagonist.Instance;
        var antagonist = Antagonist.Instance;

        if (Antagonist.Instance.IsPass == Protagonist.Instance.IsPass)
        {
            antagonist.IsPass = false;
            protagonist.IsPass = false;

            Player nextTurn = null;
            switch (antagonist.TotalCount.CompareTo(protagonist.TotalCount))
            {
                case < 0:
                    protagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
                        .ButtonPressed = true;
                    ++GameResult;
                    nextTurn = protagonist;
                    break;
                case > 0:
                    antagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
                        .ButtonPressed = true;
                    --GameResult;
                    nextTurn = antagonist;
                    break;
                case 0:
                    protagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
                        .ButtonPressed = true;
                    antagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
                        .ButtonPressed = true;
                    break;
            }

            if (Math.Abs(GameResult) == 2 || GameResult != 0 && round == 3)
            {
                gameCtrl.SetTurn(null);
                gameCtrl.MatchCompleted((GameResult > 0) ? protagonist : antagonist);
            }
            else if (GameResult == 0 && round == 3)
            {
                gameCtrl.SetTurn(null);
                gameCtrl.MatchCompleted(null);
            }
            else
            {
                CleanSpCards();
                RemoveTemporalSpCardNow();

                antagonist.OnBoard = new();
                protagonist.OnBoard = new();
                protagonist.UpdateBoard();
                antagonist.UpdateBoard();
                ++round;

                var cardList = protagonist.GetRandomCardsFromDeck(
                    Math.Min(MaxHandSize - protagonist.Hand.Count, protagonist.Deck.Count));
                if (cardList.Count > 0)
                {
                    protagonist.TakeCardsFromDeck(cardList);
                    gameCtrl.UpdateDeckTextures();
                }

                if (nextTurn is null)
                {
                    gameCtrl.InitFirstTurn();
                }
                else
                {
                    gameCtrl.SetTurn(nextTurn);
                }
            }
        }
        else
        {
            gameCtrl.SetTurn((protagonist.IsPass) ? antagonist : protagonist);
        }
    }

    protected void PutSpecialCard(CardDB.CardData cardInfo)
    {
        if (cardInfo.Strength == 0)
        {
            CleanSpCards();
        }
        else
        {
            MinCardScene cardInstance = (MinCardScene)GFieldController.MinCardScene.Instantiate();
            cardInstance.Name = cardInfo.Id.ToString();
            Vector2 cardSize = SpCardsContainer.GetParent<Control>().Size;
            int extraSpace = 3;
            cardSize.X = cardSize.X / GFieldController.MaxSpOnBoardCount - extraSpace;
            cardInstance.SetParams(cardSize, CardDB.GetCardTexturePath(cardInfo.Id), cardInfo);

            var cardContainer = new Control();
            cardContainer.AddChild(cardInstance);
            SpCardsContainer.AddChild(cardContainer);
            SpOnBoard.Add(cardInfo);
        }

        Protagonist.Instance.UpdateRowsCount();
        Protagonist.Instance.UpdateTotalCount();
        Antagonist.Instance.UpdateRowsCount();
        Antagonist.Instance.UpdateTotalCount();
    }

    protected void DisplaySelectedCard(CardDB.CardData card)
    {
        var gameCtrl = GFieldController.Instance;

        foreach (var childNode in gameCtrl.largeCardContainer.GetChildren())
        {
            gameCtrl.largeCardContainer.RemoveChild(childNode);
        }

        SlaveCardScene cardInstance = (SlaveCardScene)GFieldController.cardScene.Instantiate();
        gameCtrl.largeCardContainer.AddChild(cardInstance);
        cardInstance.SetParams(gameCtrl.largeCardContainer.Size, CardDB.GetCardTexturePath(card.Id),
            card, disconnectSignals: true);

        RemoveTemporalCardAsync(cardInstance);
    }

    private async void RemoveTemporalCardAsync(SlaveCardScene cardInstance)
    {
        int delayMlsc = 3000;
        await Task.Delay(delayMlsc);

        Control cardContainer = GFieldController.Instance.largeCardContainer;

        if (Node.IsInstanceValid(cardContainer))
        {
            if (Node.IsInstanceValid(cardInstance))
            {
                cardContainer.RemoveChild(cardInstance);
            }
        }
    }

    private void RemoveTemporalSpCardNow()
    {
        if (Node.IsInstanceValid(SpCardsContainer))
        {
            foreach (var node in SpCardsContainer.GetChildren())
            {
                if (Node.IsInstanceValid(node))
                {
                    SpCardsContainer.RemoveChild(node);
                }
            }
        }
    }
}

