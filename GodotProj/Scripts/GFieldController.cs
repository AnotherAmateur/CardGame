using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class GFieldController : Node2D, ISocketConn
{
    public static PackedScene MinCardScene = (PackedScene)GD.Load("res://MinCardScene.tscn");
    public static PackedScene cardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
    public static GFieldController Instance { get; private set; }
    public VBoxContainer RowSpCardsContainer { get; private set; }
    public HBoxContainer SpCardsContainer { get; private set; }
   
    private Control leaderCardContainerBottom;
    private Control leaderCardContainerTop;
    public Control largeCardContainer { get; private set; }

    public const int MaxSpOnBoardCount = 3;
     
    private SocketConnection socketConnection;    

    private Button passBtn;
    private bool isBoardActive;

    private Protagonist protagonist;
    private Antagonist antagonist;
    private AiPlayer aiPlayer;
    public readonly static Vector2 CardRowSize = new Vector2(1049, 127);
    private Player currentPlayer;
    private Random random;

    private GFieldController() { }

    public override void _Ready()
    {
        Instance = this;
        random = new();

        if (States.PVE)
        {
            int randNation = random.Next(CardDB.Nations.Count);
            aiPlayer = new AiPlayer(CardDB.Nations[randNation]);
            States.AntagonistLeaderCardId = CardDB.GetAllCards.Where(x =>
                x.Value.Nation == aiPlayer.Nation && x.Value.Type == CardTypes.Leader).First().Key;
        }
        else
        {
            socketConnection = SocketConnection.GetInstance(this);
        }

        passBtn = GetNode<Button>("ToPass");

        Control cardsHandContainer = GetNode<Control>("Cards");

        Control discardPileContainerTop = GetNode<Control>("DiscardPileContainerTop");
        Control discardPileContainerBottom = GetNode<Control>("DiscardPileContainerBottom");

        Control cardRowsContainerTop = GetNode<Control>("FieldRowsContainer/Top");
        Control cardRowsContainerBottom = GetNode<Control>("FieldRowsContainer/Bottom");

        Label totalCountTop = GetNode<Label>("TotalCount/VBoxContainer/Top");
        Label totalCountBottom = GetNode<Label>("TotalCount/VBoxContainer/Bottom");

        Control RowsCountTopContainer = GetNode<Control>("RowsCount/Top");
        Control RowsCountBottomContainer = GetNode<Control>("RowsCount/Bottom");

        leaderCardContainerBottom = GetNode<Control>("LeaderCardContainerBottom");
        leaderCardContainerTop = GetNode<Control>("LeaderCardContainerTop");
        RowSpCardsContainer = GetNode<VBoxContainer>("RowSpCardsContainer/VBoxContainer");
        largeCardContainer = GetNode<Control>("LargeCardContainer");
        SpCardsContainer = GetNode<HBoxContainer>("SpCardsContainer/HBox");

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
    }

    public void CardSceneEventHandler(CardEvents cardEvent, int cardId)
    {
        var cardInfo = CardDB.GetCardInfo(cardId);

        if (cardEvent is CardEvents.LeftCllick && isBoardActive)
        {
            switch (cardInfo.Type)
            {
                case CardTypes.Leader:
                    if (cardInfo.Nation == protagonist.LeaderCard.Nation)
                    {
                        // todo leader card reaction
                    }
                    break;
                default:
                    if (protagonist.Hand.Contains(cardId) && protagonist.PutCardFromHandOnBoard(cardInfo))
                    {
                        protagonist.UpdateBoard();

                        if (antagonist.IsPass is false)
                        {
                            SetTurn(antagonist);
                        }

                        if (States.PVE is false)
                        {
                            socketConnection.SendAsync(ActionTypes.CardMove, States.MasterId, cardId.ToString());
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
            cardInstance.SetParams(largeCardContainer.Size, CardDB.GetCardTexturePath(cardId),
                cardInfo, disconnectSignals: true);
        }
    }

    private void _on_Pass_pressed()
    {
        protagonist.DoPass();
        OnPassDeclare();

        if (States.PVE is false)
        {
            socketConnection.SendAsync(ActionTypes.Pass, States.MasterId, String.Empty);
        }
    }

    public void OnHandleError(string exMessage)
    {
        OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", exMessage));
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
                GetBotActionAsync();
            }
        }
        else if (actionType == ActionTypes.Pass.ToString())
        {
            antagonist.DoPass();
            OnPassDeclare();
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
        string declareWinner;

        if (winner == protagonist)
        {
            declareWinner = "Победа за вами!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.SendAsync(ActionTypes.GameOver, States.MasterId, String.Join(";", States.PlayerId, States.PlayerId));
            }
        }
        else if (winner == antagonist)
        {
            declareWinner = "Победа за оппонентом!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.SendAsync(ActionTypes.GameOver, States.MasterId, String.Join(";", States.PlayerId, States.MasterId));
            }
        }
        else
        {
            declareWinner = "Ничья!";
            if (States.MasterId != States.PlayerId && States.PVE is false)
            {
                socketConnection.SendAsync(ActionTypes.GameOver, States.MasterId, States.PlayerId.ToString());
            }
        }

        if (States.PVE is false)
        {
            socketConnection.DisconnectAsync();
        }

        var msgBox = MessageBox.Instance;
        msgBox.SetUp(declareWinner, true, true);
        GetNode("Shape").AddChild(msgBox);
    }

    private void InitializeColorRects()
    {
        var color = new Godot.Color("#988153");

        var controls = new List<Control>();
        controls.Add(leaderCardContainerTop);
        controls.Add(leaderCardContainerBottom);
        controls.Add(GetNode<Control>("Cards"));
        controls.Add(largeCardContainer);
        controls.Add(SpCardsContainer.GetParent<Control>());
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
        string texturePath = CardDB.GetFlippedCardTexturePath(
            CardDB.GetCardInfo(States.AntagonistLeaderCardId).Nation);
        topTexture.Texture = (Texture2D)GD.Load(texturePath);

        var bottomTexture = GetNode<Sprite2D>("DeckBottomImg/Texture");
        texturePath = CardDB.GetFlippedCardTexturePath(
            CardDB.GetCardInfo(States.ProtagonistLeaderCardId).Nation);
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
                socketConnection.SendAsync(ActionTypes.DeckSizeUpdated, States.MasterId, newCount);
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
        return (random.Next(0, 2) == 0) ? antagonist : protagonist;
    }

    public void SetTurn(Player player)
    {
        currentPlayer = player;
        ChangeTurnLabel(player);

        isBoardActive = player == protagonist;
        passBtn.Disabled = !isBoardActive;

        if (States.PVE && currentPlayer == antagonist)
        {
            GetBotActionAsync();
        }
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

            if (States.PVE is false)
            {
                socketConnection.SendAsync(ActionTypes.FirstPlayer, States.MasterId, firstPlayer.ToString());
            }
        }
    }

    private async void GetBotActionAsync()
    {
        if (currentPlayer == antagonist)
        {
            int delayMlsc = 500;
            await Task.Delay(delayMlsc);

            int aiAction = aiPlayer.ApplyAction(GetCurState());
            var actionType = (aiAction == (int)ActionTypes.Pass) ? ActionTypes.Pass : ActionTypes.CardMove;

            OnReceiveMessage(actionType.ToString(), "", aiAction.ToString());
        }
    }

    public string GetCurState()
    {
        List<string> curState = new();

        string protagonistDeckSize = (protagonist.Deck.Count / 3).ToString();
        curState.Add(protagonistDeckSize);

        string botDeckSize = (aiPlayer.Deck.Count / 3).ToString();
        curState.Add(botDeckSize);

        string gamesResults = Player.GameResult.ToString();
        curState.Add(gamesResults);

        string rowsTotalsPl1 = string.Join("", (protagonist.TotalsByRows.Values).Select(x => x / 3));
        curState.Add(rowsTotalsPl1);

        string rowsTotalsBot = string.Join("", (antagonist.TotalsByRows.Values).Select(x => x / 3));
        curState.Add(rowsTotalsBot);

        string spCardsOnBoard = string.Join("", Player.SpOnBoard.OrderBy(x => x.Id).Select(x => x.Id));
        curState.Add(spCardsOnBoard);

        string handSize = aiPlayer.Hand.Count.ToString();
        curState.Add(handSize);

        string handWeight = (aiPlayer.Hand.Select(x => x.Strength).Sum() / 5).ToString();
        curState.Add(handWeight);

        // todo add pass state

        return string.Join('_', curState);
    }

    private async void _on_tree_exiting()
    {
        int msecDelay = 3000;
        await Task.Delay(msecDelay);
        Instance = null;
    }
}
