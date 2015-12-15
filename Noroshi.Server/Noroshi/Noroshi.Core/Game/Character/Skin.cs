namespace Noroshi.Core.Game.Character
{
    public class Skin
    {
        byte _evolutionLevel;
        bool _isDeca;

        public Skin(byte evolutionLevel, bool isDeca)
        {
            _evolutionLevel = evolutionLevel;
            _isDeca = isDeca;
        }

        public byte GetSkinLevel()
        {
            if (_isDeca)
            {
                return _evolutionLevel;
            }
            return (byte)(_evolutionLevel < 3 ? 1 : _evolutionLevel < 5 ? 2 : 3);
        }
    }
}