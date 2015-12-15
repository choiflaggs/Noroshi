using System;
using System.Collections.Generic;
using UniLinq;

namespace Noroshi.Random
{
    public class MockRandomGenerator : IRandomGenerator
    {
        float[] _mockRandomValues;
        int _mockRandomValueIndex;

        public MockRandomGenerator(IEnumerable<float> mockRandomValues)
        {
            if (mockRandomValues.Any(f => f < 0 || 1 <= f))
            {
                throw new ArgumentOutOfRangeException();
            }
            _mockRandomValues = mockRandomValues.ToArray();
            _mockRandomValueIndex = 0;
        }
        
        public int GenerateInt(int max)
        {
            return (int)(_random() * max);
        }
        public float GenerateFloat(int max = 1)
        {
            return _random() * max;
        }
        public float GenerateFloat(float max)
        {
            return _random() * max;
        }

        public bool Lot(float probability)
        {
            return GenerateFloat() < probability;
        }

        public T Lot<T>(T[] targets)
        {
            var index = GenerateInt(targets.Count());
            return targets[index];
        }
        float _random()
        {
            var mockRandomValue = _mockRandomValues[_mockRandomValueIndex++];
            if (_mockRandomValueIndex >= _mockRandomValues.Length)
            {
                _mockRandomValueIndex = 0;
            }
            return mockRandomValue;
        }
    }
}
