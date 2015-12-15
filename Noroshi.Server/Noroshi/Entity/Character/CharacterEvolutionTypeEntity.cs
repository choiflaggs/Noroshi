using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterEvolutionTypeSchema;


namespace Noroshi.Server.Entity.Character
{
    public class CharacterEvolutionTypeEntity : AbstractDaoWrapperEntity<CharacterEvolutionTypeEntity, CharacterEvolutionTypeDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CharacterEvolutionTypeEntity> ReadAndBuildAll()
        {
            return _instantiate((new CharacterEvolutionTypeDao()).ReadAll());
        }
        public static IEnumerable<CharacterEvolutionTypeEntity> ReadAndBuildByType(ushort type)
        {
            return _instantiate((new CharacterEvolutionTypeDao()).ReadByType(type));
        }
        public static CharacterEvolutionTypeEntity ReadAndBuild(ushort type, byte level)
        {
            return _instantiate((new CharacterEvolutionTypeDao()).ReadByPK(new Schema.PrimaryKey{ Type = type, EvolutionLevel = level}));
        }
        public PossessionParam GetUsingSoulPossessionParam(uint soulId)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Soul,
                ID = soulId,
                Num = Soul,
            };
        }

        // ItemBuilder作成中に直す
        public PossessionParam GetUsingGoldPosssesionParam()
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
                Num = NecessaryGold
            };
        }

        public IEnumerable<PossessionParam> GetAllPossessionParams(uint soulId)
        {
            return new[]
            {
                GetUsingGoldPosssesionParam(),
                GetUsingSoulPossessionParam(soulId)
            };
        }

        public ushort Type => _record.Type;

        public byte EvolutionLevel => _record.EvolutionLevel;
        public ushort Soul => _record.Soul;
        public uint NecessaryGold => _record.NecessaryGold;

        public Core.WebApi.Response.Character.CharacterEvolutionType ToResponseData()
        {
            return new Core.WebApi.Response.Character.CharacterEvolutionType
            {
                Type = Type,
                EvolutionLevel = EvolutionLevel,
                Soul = Soul,
                NecessaryGold = NecessaryGold
            };
        }
    }
}