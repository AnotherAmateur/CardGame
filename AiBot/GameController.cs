using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
        public AiPlayer player { get; private set; }
        public AiPlayer targetPlayer { get; private set; }
        public AiPlayer? CurrentPlayer { get; private set; }
        public int Pl1GamesMargin { get; private set; }
        public int Round { get; private set; }
        public AiPlayer? Winner { get; private set; }
        public List<StatesLog> StatesLog { get; set; }


        public GameController(CardNations pl1Nation, CardNations pl2Nation)
        {
            Round = 1;
            Winner = null;
            player = new AiPlayer(pl1Nation);
            targetPlayer = new AiPlayer(pl2Nation);
            StatesLog = new(100);

            SetTurn(RandFirstPlayer());
        }


        private void UpdateTotals()
        {
            player.InitTotalsByRows();
            targetPlayer.InitTotalsByRows();

            foreach (var card in player.OnBoard)
            {
                player.TotalsByRows[card.type] += card.strength;
            }

            foreach (var card in targetPlayer.OnBoard)
            {
                targetPlayer.TotalsByRows[card.type] += card.strength;
            }

            foreach (var spCard in player.SpOnBoard)
            {
                switch (spCard.strength)
                {
                    case < 0:
                        targetPlayer.TotalsByRows[CardTypes.Group1] += spCard.strength *
                            targetPlayer.OnBoard.Where(x => x.type == CardTypes.Group1).Count();
                        break;
                    case > 0:
                        player.TotalsByRows[CardTypes.Group2] += spCard.strength *
                             player.OnBoard.Where(x => x.type == CardTypes.Group2).Count();
                        break;
                    case 0:
                        throw new Exception();
                }
            }

            foreach (var spCard in targetPlayer.SpOnBoard)
            {
                switch (spCard.strength)
                {
                    case < 0:
                        player.TotalsByRows[CardTypes.Group1] += spCard.strength *
                            player.OnBoard.Where(x => x.type == CardTypes.Group1).Count();
                        break;
                    case > 0:
                        targetPlayer.TotalsByRows[CardTypes.Group2] += spCard.strength *
                             targetPlayer.OnBoard.Where(x => x.type == CardTypes.Group2).Count();
                        break;
                    case 0:
                        throw new Exception();
                }
            }

            player.Total = player.TotalsByRows.Values.Sum();
            targetPlayer.Total = targetPlayer.TotalsByRows.Values.Sum();
        }

        private AiPlayer GetEnemy(AiPlayer aiPlayer)
        {
            return aiPlayer == player ? targetPlayer : player;
        }

        public void MakeMove(int action)
        {
            StatesLog.Add(new(player.Total, targetPlayer.Total, Pl1GamesMargin, CurrentPlayer, Round));

            if (action == (int)ActionTypes.Pass)
            {
                if (CurrentPlayer.IsPassed)
                {
                    throw new Exception();
                }

                DoPass();
            }
            else
            {
                var card = CardDataBase.GetCardInfo(action);

                if (card.type == CardTypes.Special && card.strength == 0)
                {
                    player.SpOnBoard.Clear();
                    targetPlayer.SpOnBoard.Clear();
                }
                else
                {
                    CurrentPlayer.PutCard(card);
                }

                var enemy = GetEnemy(CurrentPlayer);
                CurrentPlayer = enemy.IsPassed ? CurrentPlayer : enemy;
            }
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


        public void DoPass()
        {
            CurrentPlayer.IsPassed = true;
            var enemy = GetEnemy(CurrentPlayer);

            if (player.IsPassed && targetPlayer.IsPassed)
            {
                player.IsPassed = false;
                targetPlayer.IsPassed = false;

                switch (targetPlayer.Total.CompareTo(player.Total))
                {
                    case < 0:
                        ++Pl1GamesMargin;
                        SetTurn(player);
                        break;
                    case > 0:
                        --Pl1GamesMargin;
                        SetTurn(targetPlayer);
                        break;
                    case 0:
                        SetTurn(RandFirstPlayer());
                        break;
                }

                if (Math.Abs(Pl1GamesMargin) == 2 || Pl1GamesMargin != 0 && Round == 3)
                {
                    SetTurn(null);
                    Winner = (Pl1GamesMargin > 0) ? player : targetPlayer;
                    return;
                }
                else if (Pl1GamesMargin == 0 && Round == 3)
                {
                    SetTurn(null);
                    return;
                }

                if (Round < 3)
                {
                    player.NewRound();
                    targetPlayer.NewRound();
                    ++Round;
                }
            }
            else
            {
                SetTurn(player.IsPassed ? targetPlayer : player);
            }
        }

        public string GetCurState()
        {
            UpdateTotals();

            List<string> curState = new();

            string protagonistDeckSize = (player.Deck.Count() / 3).ToString();
            curState.Add(protagonistDeckSize);

            string antagonistDeckSize = (targetPlayer.Deck.Count() / 3).ToString();
            curState.Add(antagonistDeckSize);

            string gamesResults = Pl1GamesMargin.ToString();
            curState.Add(gamesResults);

            string rowsTotalsPl1 = string.Join("", (player.TotalsByRows.Values).Select(x => x / 3));
            curState.Add(rowsTotalsPl1);

            string rowsTotalsTargetPl = string.Join("", (targetPlayer.TotalsByRows.Values).Select(x => x / 3));
            curState.Add(rowsTotalsTargetPl);

            string spCardsOnBoard = string.Join("", player.SpOnBoard.Union(targetPlayer.SpOnBoard)
                .OrderBy(x => x.id).Select(x => x.id));
            curState.Add(spCardsOnBoard);

            string handSize = CurrentPlayer.Hand.Count().ToString();
            curState.Add(handSize);

            string handWeight = (CurrentPlayer.Hand.Select(x => x.strength).Sum() / 5).ToString();
            curState.Add(handWeight);

            return string.Join('_', curState);
        }

        public AiPlayer RandFirstPlayer()
        {
            var t = new Random().Next(0, 2) == 0 ? player : targetPlayer;
            return t;
        }

        public void SetTurn(AiPlayer? player)
        {
            CurrentPlayer = player;
        }

        public List<int> GetValidActions()
        {
            var actions = CurrentPlayer.Hand.Select(x => x.id).ToList();
            actions.Add((int)ActionTypes.Pass);

            return actions;
        }
    }
}