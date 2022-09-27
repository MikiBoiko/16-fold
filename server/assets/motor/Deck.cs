using System;
using System.Collections.Generic;

namespace Fold.Motor {
    public abstract class Deck {
        protected readonly int randomSeed;

        public Deck(int randomSeed) {
            this.randomSeed = randomSeed;
        }

        public abstract List<int> GenerateInitialValues(bool hasJokers, int playerStartingCardCount);
    }

    public class FrenchDeck : Deck {
        public FrenchDeck(int randomSeed) : base(randomSeed) { }

        public override List<int> GenerateInitialValues(bool hasJokers, int playerStartingCardCount) {
            Random random = new Random(randomSeed);

            List<int> initial = new List<int>() {
                2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14
            };

            List<int> initialValues = new List<int>();

            int cardsToGenerateCount = hasJokers ? playerStartingCardCount - 1 : playerStartingCardCount;
            for (int i = 0; i < cardsToGenerateCount; i++)
            {
                int randomIndex = random.Next(initial.Count);
                initialValues.Add(initial[randomIndex]);
                initial.RemoveAt(randomIndex);
            }

            return initialValues;
        }
    }
}