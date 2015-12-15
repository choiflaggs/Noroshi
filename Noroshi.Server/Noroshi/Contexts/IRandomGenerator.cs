using System;
using System.Collections.Generic;

namespace Noroshi.Server.Contexts
{
    public interface IRandomGenerator
    {
        double NextDouble();
        bool Lot(float probability);
        T Lot<T>(IEnumerable<T> candidates, Func<T, float> weight);
        T LotWithProbability<T>(IEnumerable<T> candidates, Func<T, float> probability) where T : class;
        int Next(int min, int max);
    }
}
