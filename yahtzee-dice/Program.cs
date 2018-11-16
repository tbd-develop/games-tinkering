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

        // Category
        // Score from the category
        public class ScoringRule
        {
            public string Name { get; set; }
        }

        public class ScoreCard
        {
            private IEnumerable<ScoringRule> Categories = new[]
            {
                new ScoringRule() {Name = "Aces"},
                new ScoringRule() {Name = "Twos"},
                new ScoringRule() {Name = "Threes"},
                new ScoringRule() {Name = "Fours"},
                new ScoringRule() {Name = "Fives"},
                new ScoringRule() {Name = "Sixes"},
                new ScoringRule() {Name = "Three of a Kind"},
                new ScoringRule() {Name = "Four of a Kind"},
                new ScoringRule() {Name = "Full House"},
                new ScoringRule() {Name = "Small Straight"},
                new ScoringRule() {Name = "Large Straight"},
                new ScoringRule() {Name = "Yahtzee!"},
                new ScoringRule() {Name = "Chance"}
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
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
        public int Value { get; set; }
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
                die.Value = random.Next(1, 6);
            }
        }
    }
}