using UniLinq;

namespace Noroshi.Random
{
    public class RandomGenerator : IRandomGenerator
	{
		System.Random _random;

		public RandomGenerator()
		{
			_random = new System.Random();
		}

		public int GenerateInt(int max)
		{
			return _random.Next(max);
		}
		public float GenerateFloat(int max = 1)
		{
			return (float)(_random.NextDouble() * max);
		}
		public float GenerateFloat(float max)
		{
			return (float)(_random.NextDouble() * max);
		}

        public bool Lot(float probability)
        {
            return GenerateFloat() < probability;
        }
		public T Lot<T>(T[] targets)
		{
			var index = _random.Next(targets.Count());
			return targets[index];
		}
	}
}
