using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Entity.Possession
{
    public class Character : IPossessionObject
    {
        readonly CharacterEntity _character;

        public Character(CharacterEntity character, uint possessingNum)
        {
            _character = character;
            PossessingNum = possessingNum;
        }

        public PossessionCategory Category => PossessionCategory.Character;
        public uint ID => _character.ID;
        public uint Num => 1;

        // TODO
        public string TextKey => _character.TextKey;
        public uint PossessingNum { get; }

        public Core.WebApi.Response.Possession.PossessionObject ToResponseData()
        {
            return new Core.WebApi.Response.Possession.PossessionObject
            {
                Category = (byte)Category,
                ID = ID,
                Num = Num,

                Name = TextKey,
                PossessingNum = PossessingNum,
            };
        }
    }
}
