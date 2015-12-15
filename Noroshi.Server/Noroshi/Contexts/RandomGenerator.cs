using System;
using System.Collections.Generic;
using System.Linq;

namespace Noroshi.Server.Contexts
{
    public class RandomGenerator : IRandomGenerator
    {
        Random _random;

        public RandomGenerator()
        {
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        public bool Lot(float probability)
        {
            return NextDouble() < probability;
        }

        public T Lot<T>(IEnumerable<T> candidates, Func<T, float> weight)
        {
            var random = NextDouble() * candidates.Sum(c => weight(c));
            var current = 0f;
            foreach (var candidate in candidates)
            {
                current += weight(candidate);
                if (current > random) return candidate;
            }
            return candidates.Last();
        }

        public T LotWithProbability<T>(IEnumerable<T> candidates, Func<T, float> probability) where T : class
        {
            var sum = candidates.Sum(c => probability(c));
            if (!Lot(sum)) return null;
            var random = NextDouble() * sum;
            var current = 0f;
            foreach (var candidate in candidates)
            {
                current += probability(candidate);
                if (current > random) return candidate;
            }
            return candidates.Last();
        }

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}