using CardGameProj.Scripts;

namespace AiBot
{
    public struct StatesLog
    {
        public int EnemyTotal { get; private set; }
        public int SelfTotal { get; private set; }
        public int SelfGamesRslt { get; private set; }
        public int Round { get; private set; }
        public int HandCount { get; private set; }
        public bool EnemePassed { get; private set; }

        public StatesLog(int enemyTotal, int selfTotal, int selfGamesRslt, int round, int handCount, bool enemePassed)
        {
            EnemyTotal = enemyTotal;
            SelfTotal = selfTotal;
            SelfGamesRslt = selfGamesRslt;
            Round = round;
            HandCount = handCount;
            EnemePassed = enemePassed;
        }
    }

    public class GameController
    {
        public const int MaxHandSize = 10;
        public AiPlayer FirstPl { get; private set; }
        public AiPlayer SecondPl { get; private set; }
        public AiPlayer? CurrentPlayer { get; private set; }
        public int Round { get; private set; }
        public AiPlayer? Winner { get; private set; }
        public List<CardDB.CardData> SpOnBoard { get; private set; }
        public GameController Instance { get; private set; }
        private Random random;
        public const int MaxSpOnBoardCount = 3;

        public GameController(CardNations pl1Nation, CardNations pl2Nation)
        {
            Instance = this;
            SpOnBoard = new();
            random = new();
            Round = 1;
            Winner = null;
            FirstPl = new AiPlayer(pl1Nation, this);
            SecondPl = new AiPlayer(pl2Nation, this);

            SetTurn(RandFirstPlayer());
        }

        private AiPlayer GetEnemy(AiPlayer aiPlayer)
        {
            return aiPlayer == FirstPl ? SecondPl : FirstPl;
        }

        public void MakeMove(int action)
        {
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
                    CurrentPlayer.Hand.Remove(card);
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

            if (FirstPl.IsPass && SecondPl.IsPass)
            {
                FirstPl.IsPass = false;
                SecondPl.IsPass = false;

                switch (SecondPl.Total.CompareTo(FirstPl.Total))
                {
                    case < 0:
                        ++FirstPl.PlGamesMargin;
                        --SecondPl.PlGamesMargin;
                        SetTurn(FirstPl);
                        break;
                    case > 0:
                        --FirstPl.PlGamesMargin;
                        ++SecondPl.PlGamesMargin;
                        SetTurn(SecondPl);
                        break;
                    case 0:
                        SetTurn(RandFirstPlayer());
                        break;
                }

                if (Math.Abs(FirstPl.PlGamesMargin) == 2 || SecondPl.PlGamesMargin != 0 && Round == 3)
                {
                    SetTurn(null);
                    Winner = (FirstPl.PlGamesMargin > SecondPl.PlGamesMargin) ? FirstPl : SecondPl;
                    return;
                }
                else if (FirstPl.PlGamesMargin == 0 && Round == 3)
                {
                    SetTurn(null);
                    return;
                }

                if (Round < 3)
                {
                    FirstPl.NewRound();
                    SecondPl.NewRound();
                    ++Round;
                }
            }
            else
            {
                SetTurn(FirstPl.IsPass ? SecondPl : FirstPl);
            }
        }

        public AiPlayer RandFirstPlayer()
        {
            return random.Next(0, 2) == 0 ? FirstPl : SecondPl;
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

        public string GetCurStateString()
        {
            FirstPl.UpdateTotals();
            SecondPl.UpdateTotals();

            var enemyPl = GetEnemy(CurrentPlayer);

            List<string> curState = new();

            string pl1DeckSize = (enemyPl.Deck.Count / 2).ToString();
            curState.Add(pl1DeckSize);

            string targetPlayerDeckSize = (CurrentPlayer.Deck.Count / 2).ToString();
            curState.Add(targetPlayerDeckSize);

            string gamesEnemyResults = enemyPl.PlGamesMargin.ToString();
            curState.Add(gamesEnemyResults);

            string rowsTotalsPl1 = string.Join("-", (enemyPl.TotalsByRows.Values).Select(x => x / 2));
            curState.Add(rowsTotalsPl1);

            string rowsTotalsTargetPl = string.Join("-", (CurrentPlayer.TotalsByRows.Values).Select(x => x / 2));
            curState.Add(rowsTotalsTargetPl);

            string spCardsOnBoard = string.Join("-", SpOnBoard.OrderBy(x => x.Id).Select(x => x.Id));
            curState.Add(spCardsOnBoard);

            string handSize = CurrentPlayer.Hand.Count.ToString();
            curState.Add(handSize);

            string handWeight = (CurrentPlayer.Hand.Select(x => x.Strength).Sum() / 2).ToString();
            curState.Add(handWeight);

            string enemyPassed = enemyPl.IsPass.ToString();
            curState.Add(enemyPassed);

            return string.Join('_', curState);
        }

        public StatesLog GetCurState(AiPlayer pl)
        {
            var enemy = GetEnemy(pl);
            return new(enemy.Total, pl.Total, pl.PlGamesMargin, Round, pl.Hand.Count, enemy.IsPass);
        }
    }
}