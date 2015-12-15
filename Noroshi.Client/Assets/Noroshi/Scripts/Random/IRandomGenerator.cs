namespace Noroshi.Random
{
    public interface IRandomGenerator
    {
        int GenerateInt(int max);
        float GenerateFloat(int max = 1);
        float GenerateFloat(float max);
        bool Lot(float probability);
        T Lot<T>(T[] targets);
    }
}
