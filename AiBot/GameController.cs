using CardGameProj.Scripts;

namespace AiBot
{
    public struct StatesLog
    {
        public int Pl1Total { get; private set; }
        public int Pl2Total { get; private set; }
        public int Pl1GamesMargin { get; private set; }
        public AiPlayer? Mover { get; private set; }
        public int Round { get; private set; }

        public StatesLog(int pl1Total, int pl2Total, int pl1GamesMargin, AiPlayer mover, int round)
        {
            Pl1Total = pl1Total;
            Pl2Total = pl2Total;
            Pl1GamesMargin = pl1GamesMargin;
            Mover = mover ?? throw new ArgumentNullException(nameof(mover));
            Round = round;
        }
    }


    public class GameController
    {
        public const int MaxHandSize = 10;
        public AiPlayer Player { get; private set; }
        public AiPlayer TargetPlayer { get; private set; }
        public AiPlayer? CurrentPlayer { get; private set; }
        public int Pl1GamesMargin { get; private set; }
        public int Round { get; private set; }
        public AiPlayer? Winner { get; private set; }
        public List<CardDB.CardData> SpOnBoard { get; private set; }
        public GameController Instance { get; private set; }         
        public List<StatesLog> StatesLog { get; set; }
        private Random random;
        public const int MaxSpOnBoardCount = 3;

        public GameController(CardNations pl1Nation, CardNations pl2Nation)
        {
            Instance = this;
            SpOnBoard = new();
            random = new();
            Round = 1;
            Winner = null;
            Player = new AiPlayer(pl1Nation, this);
            TargetPlayer = new AiPlayer(pl2Nation, this);
            StatesLog = new(100);

            SetTurn(RandFirstPlayer());
        }

        private AiPlayer GetEnemy(AiPlayer aiPlayer)
        {
            return aiPlayer == Player ? TargetPlayer : Player;
        }

        public void MakeMove(int action)
        {
            StatesLog.Add(new(Player.Total, TargetPlayer.Total, Pl1GamesMargin, CurrentPlayer, Round));

            if (action == (int)ActionTypes.Pass)
            {
                if (CurrentPlayer.IsPass)
                {
                    throw new Exception("Double pass");
                }

                DoPass();
            }
            else
            {
                var card = CardDB.GetCardInfo(action);

                if (card.Type == CardTypes.Special && card.Range == CardRanges.OutOfRange)
                {
                   SpOnBoard.Clear();
                }
                else
                {
                    CurrentPlayer.PutCard(card);
                }

                var enemy = GetEnemy(CurrentPlayer);
                CurrentPlayer = enemy.IsPass ? CurrentPlayer : enemy;
            }
        }

        public void DoPass()
        {
            CurrentPlayer.IsPass = true;

            if (Player.IsPass && TargetPlayer.IsPass)
            {
                Player.IsPass = false;
                TargetPlayer.IsPass = false;

                switch (TargetPlayer.Total.CompareTo(Player.Total))
                {
                    case < 0:
                        ++Pl1GamesMargin;
                        SetTurn(Player);
                        break;
                    case > 0:
                        --Pl1GamesMargin;
                        SetTurn(TargetPlayer);
                        break;
                    case 0:
                        SetTurn(RandFirstPlayer());
                        break;
                }

                if (Math.Abs(Pl1GamesMargin) == 2 || Pl1GamesMargin != 0 && Round == 3)
                {
                    SetTurn(null);
                    Winner = (Pl1GamesMargin > 0) ? Player : TargetPlayer;
                    return;
                }
                else if (Pl1GamesMargin == 0 && Round == 3)
                {
                    SetTurn(null);
                    return;
                }

                if (Round < 3)
                {
                    Player.NewRound();
                    TargetPlayer.NewRound();
                    ++Round;
                }
            }
            else
            {
                SetTurn(Player.IsPass ? TargetPlayer : Player);
            }
        }

        public string GetCurState()
        {
            Player.UpdateTotals();
            TargetPlayer.UpdateTotals();

            List<string> curState = new();

            string pl1DeckSize = (Player.Deck.Count / 3).ToString();
            curState.Add(pl1DeckSize);

            string targetPlayerDeckSize = (TargetPlayer.Deck.Count / 3).ToString();
            curState.Add(targetPlayerDeckSize);

            string gamesResults = Pl1GamesMargin.ToString();
            curState.Add(gamesResults);

            string rowsTotalsPl1 = string.Join("", (Player.TotalsByRows.Values).Select(x => x / 3));
            curState.Add(rowsTotalsPl1);

            string rowsTotalsTargetPl = string.Join("", (TargetPlayer.TotalsByRows.Values).Select(x => x / 3));
            curState.Add(rowsTotalsTargetPl);

            string spCardsOnBoard = string.Join("", SpOnBoard.OrderBy(x => x.Id).Select(x => x.Id));
            curState.Add(spCardsOnBoard);

            string handSize = CurrentPlayer.Hand.Count.ToString();
            curState.Add(handSize);

            string handWeight = (CurrentPlayer.Hand.Select(x => x.Strength).Sum() / 5).ToString();
            curState.Add(handWeight);

            string plPassed = GetEnemy(CurrentPlayer).IsPass.ToString();
            curState.Add(plPassed);

            return string.Join('_', curState);
        }

        public AiPlayer RandFirstPlayer()
        {
            return random.Next(0, 2) == 0 ? Player : TargetPlayer;
        }

        public void SetTurn(AiPlayer? player)
        {
            CurrentPlayer = player;
        }

        public List<int> GetValidActions()
        {
            IEnumerable<CardDB.CardData> actions;

            if (SpOnBoard.Count == MaxSpOnBoardCount)
            {
                actions = CurrentPlayer.Hand.Where(x =>
                    x.Type != CardTypes.Special || x.Range == CardRanges.OutOfRange);
            }
            else
            {
                actions = CurrentPlayer.Hand.Where(x =>
                    SpOnBoard.Select(y => y.Range).Contains(x.Range) is false);
            }

            var result = actions.Select(x => x.Id).ToList();
            result.Add((int)ActionTypes.Pass);

            return result;
        }


        //public double LastMoveGetReward()
        //{

        //    if (CurrentPlayer is null)
        //    {
        //        if (Winner is null)
        //        {
        //            int matchReward = 150;
        //            return matchReward;
        //        }

        //        int winReward = int.MaxValue;
        //        int losReward = int.MinValue;

        //        return mover == Winner ? winReward : losReward;
        //    }
        //    if (initRound != Round)
        //    {
        //        if (initPl1GamesMargin != Pl1GamesMargin)
        //        {
        //            int dif = Pl1GamesMargin - initPl1GamesMargin;
        //            int multiplier = 100;

        //            return player1 == mover ? dif * multiplier : dif * multiplier * -1;
        //        }
        //    }
        //    else
        //    {
        //        int dif = initPl1Total - initPl2Total;
        //        int multiplier = 1;

        //        return player1 == mover ? dif * multiplier : dif * multiplier * -1;
        //    }

        //    return 0;

        //}
    }
}