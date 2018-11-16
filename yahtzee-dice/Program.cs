using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace yahtzee_dice
{
    public class Yahtzee
    {
        private DiceCup _cup;

        private const int NumberOfDice = 5;

        public Yahtzee()
        {
            _cup = new DiceCup(NumberOfDice.Dice());
        }

        public void PlayRound()
        {
            _cup.Shuffle();

            ScoreCard scoring = new ScoreCard();

            var results = scoring.Score(_cup.Dice);

            Console.WriteLine("Dice: {0}", string.Join(",", _cup.Dice.OrderBy(k => k.Value).Select(s => s.Value)));

            foreach (var result in results)
            {
                Console.WriteLine("{0} would score {1}", result.Name, result.Scoring(_cup.Dice));
            }
        }

        // Category
        // Score from the category
        public class ScoringRule
        {
            public string Name { get; set; }
            public Func<IEnumerable<Die>, byte> Scoring { get; set; }
        }

        public class ScoreCard
        {
            private static readonly byte[][] LowerSequences =
            {
                new byte[] {1, 2, 3, 4}, new byte[] {2, 3, 4, 5}, new byte[] {3, 4, 5, 6}
            };

            private static readonly byte[][] UpperSequences =
            {
                new byte[] {1, 2, 3, 4, 5}, new byte[] {2, 3, 4, 5, 6}
            };

            private IEnumerable<ScoringRule> Rules = new[]
            {
                new ScoringRule {Name = "Aces", Scoring = dice => (byte) dice.Count(d => d.Value == 1)},
                new ScoringRule {Name = "Twos", Scoring = dice => (byte) dice.Count(d => d.Value == 2)},
                new ScoringRule {Name = "Threes", Scoring = dice => (byte) dice.Count(d => d.Value == 3)},
                new ScoringRule {Name = "Fours", Scoring = dice => (byte) dice.Count(d => d.Value == 4)},
                new ScoringRule {Name = "Fives", Scoring = dice => (byte) dice.Count(d => d.Value == 5)},
                new ScoringRule {Name = "Sixes", Scoring = dice => (byte) dice.Count(d => d.Value == 6)},
                new ScoringRule
                {
                    Name = "Three of a Kind", Scoring = dice => (from x in dice
                        group x by x.Value
                        into g
                        where g.Count() >= 3
                        select (byte) g.Take(3).Sum(k => k.Value)).SingleOrDefault()
                },
                new ScoringRule
                {
                    Name = "Four of a Kind", Scoring = dice => (from x in dice
                        group x by x.Value
                        into g
                        where g.Count() >= 4
                        select (byte) g.Take(4).Sum(k => k.Value)).SingleOrDefault()
                },
                new ScoringRule
                {
                    Name = "Full House", Scoring = dice =>
                    {
                        var groups = (from x in dice
                            group x by x.Value
                            into g
                            select g).ToList();

                        return (byte) (groups.Any(k => k.Count() == 2) &&
                                       groups.Any(k => k.Count() == 3)
                            ? 25
                            : 0);
                    }
                },
                new ScoringRule
                {
                    Name = "Small Straight",
                    Scoring = dice =>
                    {
                        return (byte) (LowerSequences.Any(s => !s.Except(dice.Select(d => d.Value).ToArray()).Any())
                            ? 30
                            : 0);
                    }
                },
                new ScoringRule
                {
                    Name = "Large Straight", Scoring = dice =>
                    {
                        return (byte) (UpperSequences.Any(s => !s.Except(dice.Select(d => d.Value).ToArray()).Any())
                            ? 40
                            : 0);
                    }
                },
                new ScoringRule
                {
                    Name = "Yahtzee!",
                    Scoring = dice => (byte) (dice.All(k => k.Value == dice.ElementAt(0).Value) ? 50 : 0)
                },
                new ScoringRule {Name = "Chance", Scoring = dice => (byte) dice.Sum(d => d.Value)}
            };

            public IEnumerable<ScoringRule> Score(IEnumerable<Die> dice)
            {
                var matching = from rule in Rules
                    where rule.Scoring(dice) > 0
                    select rule;

                return matching;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var yahtzee = new Yahtzee();

            yahtzee.PlayRound();

            Console.ReadLine();
        }
    }

    public static class GameExtensions
    {
        public static IEnumerable<Die> Dice(this int numberOfDice)
        {
            for (int i = 0; i < numberOfDice; i++)
            {
                yield return new Die();
            }
        }
    }

    public class Die
    {
        public byte Value { get; set; }
    }

    public class DiceCup
    {
        private readonly IList<Die> _dice;
        public ReadOnlyCollection<Die> Dice => new ReadOnlyCollection<Die>(_dice);

        private DiceCup()
        {
            _dice = new List<Die>();
        }

        public DiceCup(IEnumerable<Die> startingDice) : this()
        {
            _dice = new List<Die>(startingDice);
        }

        public void EmptyCup()
        {
            _dice.Clear();
        }

        public void DropInDie(params Die[] dice)
        {
            foreach (var die in dice)
            {
                _dice.Add(die);
            }
        }

        public void Shuffle()
        {
            var random = new Random();

            foreach (var die in _dice)
            {
                die.Value = (byte) random.Next(1, 6);
            }
        }
    }
}