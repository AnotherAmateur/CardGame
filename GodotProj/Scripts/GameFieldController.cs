using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameFieldController : Node2D, ISocketConn
{
    public static PackedScene MinCardScene = (PackedScene)GD.Load("res://MinCardScene.tscn");
    private PackedScene cardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
    public static GameFieldController Instance { get; private set; }
    public VBoxContainer SpecialCardsContainer { get; private set; }
    public Control TemporalSpCardContainer { get; private set; }
    protected Control leaderCardContainerBottom;
    protected Control leaderCardContainerTop;
    private Control largeCardContainer;
    private Protagonist protagonist;
    private Antagonist antagonist;
    private SocketConnection socketConnection;
    private GameFieldController() { }
    private Button passBtn;
    private bool isBoardActive;
    private AiPlayer aiPlayer;
    public Timer timer { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        if (States.PVE)
        {           
            int randNation = Random.Shared.Next(CardDataBase.Nations.Count);
            aiPlayer = new AiPlayer(CardNations.Confucius);
            //aiPlayer = new AiPlayer(CardDataBase.Nations[randNation]);
            States.AntagonistLeaderCardId = CardDataBase.GetAllCards.Where(x =>
                x.Value.nation == aiPlayer.Nation && x.Value.type == CardTypes.Leader).First().Key;
        }
        else
        {
            socketConnection = SocketConnection.GetInstance(this);
        }

        passBtn = GetNode<Button>("ToPass");

        Control cardsHandContainer = GetNode<Control>("Cards");

        Control discardPileContainerTop = GetNode<Control>("DiscardPileContainerTop");
        Control discardPileContainerBottom = GetNode<Control>("DiscardPileContainerBottom");

        Control cardRowsContainerTop = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Top");
        Control cardRowsContainerBottom = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Bottom");

        Label totalCountTop = GetNode<Label>("TotalCount/VBoxContainer/Top");
        Label totalCountBottom = GetNode<Label>("TotalCount/VBoxContainer/Bottom");

        Control RowsCountTopContainer = GetNode<Control>("RowsCount").GetNode<Control>("Top");
        Control RowsCountBottomContainer = GetNode<Control>("RowsCount").GetNode<Control>("Bottom");

        leaderCardContainerBottom = GetNode<Control>("LeaderCardContainerBottom");
        leaderCardContainerTop = GetNode<Control>("LeaderCardContainerTop");
        SpecialCardsContainer = GetNode<VBoxContainer>("SpecialCardsContainer/VBoxContainer");
        largeCardContainer = GetNode<Control>("LargeCardContainer");
        TemporalSpCardContainer = GetNode<Control>("TemporalSpCardContainer");

        var roundVBoxTop = GetNode<HBoxContainer>("TotalCount/VBoxContainer/TopRoundCount");
        var roundVBoxBottom = GetNode<HBoxContainer>("TotalCount/VBoxContainer/BottomRoundCount");

        antagonist = new(States.AntagonistLeaderCardId, cardRowsContainerTop, leaderCardContainerTop,
            discardPileContainerTop, totalCountTop, RowsCountTopContainer, roundVBoxTop);

        protagonist = new(States.ProtagonistLeaderCardId, CardSelectionMenu.SelectedCards,
            cardRowsContainerBottom, cardsHandContainer, leaderCardContainerBottom,
            discardPileContainerBottom, totalCountBottom, RowsCountBottomContainer, roundVBoxBottom);

        protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(
            Math.Min(Player.MaxHandSize, CardSelectionMenu.SelectedCards.Count)));

        UpdateDeckTextures();
        UpdateDeckSize();
        InitializeColorRects();
        InitFirstTurn();

        timer = new Godot.Timer();
        AddChild(timer);
    }

    public void CardSceneEventHandler(CardEvents cardEvent, int cardId)
    {
        var cardInfo = CardDataBase.GetCardInfo(cardId);

        if (cardEvent is CardEvents.LeftCllick && isBoardActive)
        {
            switch (cardInfo.type)
            {
                case CardTypes.Leader:
                    // nothing for now
                    break;
                default:
                    if (protagonist.Hand.Contains(cardId))
                    {
                        if (antagonist.IsPass is false)
                        {
                            SetTurn(antagonist);
                        }

                        protagonist.PutCardFromHandOnBoard(cardInfo);

                        if (States.PVE)
                        {
                            GetBotAction();
                        }
                        else
                        {
                            socketConnection.Send(ActionTypes.CardMove, States.MasterId, cardId.ToString());
                        }
                    }
                    break;
            }
        }
        else if (cardEvent is CardEvents.RightClick)
        {
            foreach (var childNode in largeCardContainer.GetChildren())
            {
                largeCardContainer.RemoveChild(childNode);
            }

            SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate();
            largeCardContainer.AddChild(cardInstance);
            cardInstance.SetParams(largeCardContainer.Size, CardDataBase.GetCardTexturePath(cardId),
                cardInfo, disconnectSignals: true);
        }
    }

    private void _on_Pass_pressed()
    {
        passBtn.Disabled = (antagonist.IsPass) ? false : true;
        protagonist.DoPass();
        OnPassDeclare();

        if (States.PVE)
        {
            GetBotAction();
        }
        else
        {
            socketConnection.Send(ActionTypes.Pass, States.MasterId, String.Empty);
        }
    }

    public void OnHandleError(string exMessage)
    {
        //OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", exMessage));
    }

    public void OnReceiveMessage(string actionType, string masterId, string message)
    {
        if (actionType == ActionTypes.CardMove.ToString())
        {
            antagonist.PutCardOnBoard(int.Parse(message));
            protagonist.UpdateBoard();

            if (protagonist.IsPass is false)
            {
                SetTurn(protagonist);
            }
            else
            {
                GetBotAction();
            }
        }
        else if (actionType == ActionTypes.Pass.ToString())
        {
            antagonist.DoPass();
            OnPassDeclare();
            passBtn.Disabled = false;
        }
        else if (actionType == ActionTypes.FirstPlayer.ToString())
        {
            SetTurn((message.CompareTo(protagonist.ToString()) == 0) ? antagonist : protagonist);
        }
        else if (actionType == ActionTypes.DeckSizeUpdated.ToString())
        {
            UpdateDeckSize(message);
        }
        else
        {
            OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", actionType, masterId, message));
        }
    }

    public void MatchCompleted(Player winner)
    {
        string declareWinner = "";

        if (winner == protagonist)
        {
            declareWinner = "Победа за вами!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.Send(ActionTypes.GameOver, States.MasterId, String.Join(";", States.PlayerId, States.PlayerId));
            }
        }
        else if (winner == antagonist)
        {
            declareWinner = "Победа за оппонентом!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.Send(ActionTypes.GameOver, States.MasterId, String.Join(";", States.PlayerId, States.MasterId));
            }
        }
        else
        {
            declareWinner = "Ничья!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.Send(ActionTypes.GameOver, States.MasterId, States.PlayerId.ToString());
            }
        }

        if (States.PVE is false)
        {
            socketConnection.Disconnect();
        }
        var msgBox = MessageBox.Instance;
        msgBox.SetUp(declareWinner, true, true);
        GetNode("Shape").AddChild(msgBox);
    }

    private void InitializeColorRects()
    {
        var color = new Godot.Color("#988153");

        var controls = new List<Control>();
        controls.Add(GetNode<Control>("LeaderCardContainerTop"));
        controls.Add(GetNode<Control>("LeaderCardContainerBottom"));
        controls.Add(GetNode<Control>("Cards"));
        controls.Add(GetNode<Control>("LargeCardContainer"));
        controls.Add(GetNode<Control>("TemporalSpCardContainer"));
        controls.Add(GetNode<Control>("DeckTopImg"));
        controls.Add(GetNode<Control>("DeckBottomImg"));

        float rowHeight = 127f;
        float containerOffsetX = 524f;
        float topMargin = GetNode<Control>("FieldRowsContainer").Position.Y;
        var rowSize = new Vector2(GetNode<VBoxContainer>("FieldRowsContainer/Top").Size.X, rowHeight);
        for (int i = 1; i <= 3; i++)
        {
            var offset = new Vector2(containerOffsetX, topMargin + (i - 1) * (4 + rowHeight));
            var item = GetNode<Control>("FieldRowsContainer/Top/Row" + i.ToString());
            var t = new ColorRect();
            t.Size = rowSize;
            t.Position = new Vector2(item.Position.X + offset.X, item.Position.Y + offset.Y);
            t.Color = color;
            t.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(t);

            item = GetNode<Control>("FieldRowsContainer/Bottom/Row" + i.ToString());
            offset = new Vector2(containerOffsetX, topMargin + 397 + (i - 1) * (4 + rowHeight));
            t = new ColorRect();
            t.Size = rowSize;
            t.Position = new Vector2(item.Position.X + offset.X, item.Position.Y + offset.Y);
            t.Color = color;
            t.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(t);
        }

        const int margin = 12;
        foreach (var item in controls)
        {
            var t = new ColorRect();
            t.Size = new Vector2(item.Size.X + margin, item.Size.Y + margin);
            t.Position = new Vector2(item.Position.X - margin / 2, item.Position.Y - margin / 2);
            t.Color = color;
            t.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(t);
        }
    }

    public void UpdateDeckTextures()
    {
        var topTexture = GetNode<Sprite2D>("DeckTopImg/Texture");
        string texturePath = CardDataBase.GetFlippedCardTexturePath(
            CardDataBase.GetCardInfo(States.AntagonistLeaderCardId).nation);
        topTexture.Texture = (Texture2D)GD.Load(texturePath);

        var bottomTexture = GetNode<Sprite2D>("DeckBottomImg/Texture");
        texturePath = CardDataBase.GetFlippedCardTexturePath(
            CardDataBase.GetCardInfo(States.ProtagonistLeaderCardId).nation);
        bottomTexture.Texture = (Texture2D)GD.Load(texturePath);
    }

    public void UpdateDeckSize(string newCount = null)
    {
        if (newCount is null)
        {
            newCount = protagonist.Deck.Count.ToString();
            GetNode<Label>("DeckBottomImg/DeckSizeBottom").Text = newCount;
            if (States.PVE is false)
            {
                socketConnection.Send(ActionTypes.DeckSizeUpdated, States.MasterId, newCount);
            }
        }
        else
        {
            GetNode<Label>("DeckTopImg/DeckSizeTop").Text = newCount;
        }
    }

    private void ChangeTurnLabel(Player player)
    {
        string antTurn = "Ход оппонента";
        string protTurn = "Ваш ход";
        var label = GetNode<Label>("TurnLabel");

        if (player == protagonist)
        {
            label.Text = protTurn;
            label.AddThemeColorOverride("", new Godot.Color("#ffffff"));
        }
        else if (player == antagonist)
        {
            label.Text = antTurn;
            label.AddThemeColorOverride("", new Godot.Color("#a30003"));
        }
        else
        {
            label.Text = "";
        }
    }

    public Player RandFirstPlayer()
    {
        return (Random.Shared.Next(0, 2) == 0) ? antagonist : protagonist;
    }

    public void SetTurn(Player player)
    {
        ChangeTurnLabel(player);

        isBoardActive = (player == protagonist) ? true : false;
    }

    public void OnPassDeclare()
    {
        string pass = "ПАС";
        GetNode<Label>("LeaderCardContainerTop/Label").Text = (antagonist.IsPass) ? pass : "";
        GetNode<Label>("LeaderCardContainerBottom/Label").Text = (protagonist.IsPass) ? pass : "";
    }

    public void InitFirstTurn()
    {
        if (States.MasterId == States.PlayerId)
        {
            Player firstPlayer = RandFirstPlayer();
            SetTurn(firstPlayer);

            if (States.PVE)
            {
                if (firstPlayer == antagonist)
                {
                    GetBotAction();
                }

                return;
            }            

            socketConnection.Send(ActionTypes.FirstPlayer, States.MasterId, firstPlayer.ToString());
        }
    }

    private void GetBotAction()
    {
        int aiAction = aiPlayer.ApplyAction(GetCurState());
        var actionType = (aiAction == (int)ActionTypes.Pass) ? ActionTypes.Pass : ActionTypes.CardMove;

        OnReceiveMessage(actionType.ToString(), "", aiAction.ToString());
    }

    public string GetCurState()
    {
        List<string> curState = new();

        string protagonistDeckSize = (protagonist.Deck.Count / 3).ToString();
        curState.Add(protagonistDeckSize);

        string botDeckSize = (aiPlayer.Deck.Count / 3).ToString();
        curState.Add(botDeckSize);

        string gamesResults = Player.gameResult.ToString();
        curState.Add(gamesResults);

        string rowsTotalsPl1 = string.Join("", (protagonist.TotalsByRows.Values).Select(x => x / 3));
        curState.Add(rowsTotalsPl1);

        string rowsTotalsBot = string.Join("", (antagonist.TotalsByRows.Values).Select(x => x / 3));
        curState.Add(rowsTotalsBot);

        string spCardsOnBoard = string.Join("", protagonist.SpOnBoard.Union(antagonist.SpOnBoard)
                .OrderBy(x => x.id).Select(x => x.id));
        curState.Add(spCardsOnBoard);

        string handSize = aiPlayer.Hand.Count().ToString();
        curState.Add(handSize);

        string handWeight = (aiPlayer.Hand.Select(x => x.strength).Sum() / 5).ToString();
        curState.Add(handWeight);

        // todo add pass state

        return string.Join('_', curState);
    }
}
