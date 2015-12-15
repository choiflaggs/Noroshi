using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Story;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Story;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryStageSchema;

namespace Noroshi.Server.Entity
{
    public class StoryStageEntity : AbstractDaoWrapperEntity<StoryStageEntity, StoryStageDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<StoryStageEntity> ReadAndBuildAll()
        {
            return _instantiate((new StoryStageDao()).ReadAll());
        }
        public static StoryStageEntity ReadAndBuild(uint id)
        {
            return ReadAndBuild(new Schema.PrimaryKey { ID = id });
        }

        public static IEnumerable<StoryStageEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }));
        }


        public static IEnumerable<StoryStageEntity> ReadAndBuildByEpisodeID(uint episodeId)
        {
            return _instantiate((new StoryStageDao()).ReadByEpisodeID(episodeId));
        }

        public uint ID => _record.ID;
        public uint EpisodeID => _record.EpisodeID;
        public Enums.StageType Type => (Enums.StageType)_record.Type;
        public uint BattleID => _record.BattleID;
        public bool IsFixedParty => _record.IsFixedParty > 0;
        public uint No => _record.No;
        public uint[] FixedCharacterIDs => new[]
        {
            _record.FixedCharacterID1,
            _record.FixedCharacterID2,
            _record.FixedCharacterID3,
            _record.FixedCharacterID4,
            _record.FixedCharacterID5
        };
        public uint[] CpuCharacterIDs => new[]
        {
            _record.CpuCharacterID1,
            _record.CpuCharacterID2,
            _record.CpuCharacterID3,
            _record.CpuCharacterID4,
            _record.CpuCharacterID5
        };
        public string TextKey => string.IsNullOrEmpty(_record.TextKey) ? "" : "Master.StoryStage." + _record.TextKey;
        public ushort Stamina => _record.Stamina;

        public Core.WebApi.Response.Story.StoryStage ToResponseData(CpuBattleEntity battle, PossessionManager possessionManager)
        {
            return new Core.WebApi.Response.Story.StoryStage
            {
                ID = ID,
                EpisodeID = EpisodeID,
                Type = Type,
                BattleID = BattleID,
                TextKey = TextKey,
                Stamina = Stamina,
                FixedCharacterIDs = FixedCharacterIDs,
                CpuCharacterIDs = CpuCharacterIDs,
                Battle = battle.ToResponseData(possessionManager)
            };
        }
    }
}
