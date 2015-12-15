using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.SoulSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class SoulEntity : AbstractDaoWrapperEntity<SoulEntity, SoulDao, Schema.PrimaryKey, Schema.Record>
    {
        public static SoulEntity ReadAndBuild(uint soulId)
        {
            return ReadAndBuildMulti(new[] { soulId }).FirstOrDefault();
        }
        public static IEnumerable<SoulEntity> ReadAndBuildMulti(IEnumerable<uint> soulIds)
        {
            return _loadAssociatedRecords(ReadAndBuildMulti(soulIds.Select(id => new Schema.PrimaryKey { ID = id })));
        }
        public static IEnumerable<SoulEntity> ReadAndBuildAll()
        {
            return _loadAssociatedRecords(_instantiate((new SoulDao()).ReadAll()));
        }

        public static SoulEntity ReadAndBuildByCharacterID(uint characterId)
        {
            return ReadAndBuildMultiByCharacterIDs(new[] { characterId }).FirstOrDefault();
        }
        public static IEnumerable<SoulEntity> ReadAndBuildMultiByCharacterIDs(IEnumerable<uint> characterIds)
        {
            return _loadAssociatedRecords(_instantiate((new SoulDao()).ReadByCharacterIDs(characterIds)));
        }
        static IEnumerable<SoulEntity> _loadAssociatedRecords(IEnumerable<SoulEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var itemMap = (new ItemDao()).ReadMultiByPKs(entities.Select(soul => new ItemSchema.PrimaryKey { ID = soul.SoulID })).ToDictionary(item => item.ID);
            return entities.Select(entity =>
            {
                entity._setItemRecord(itemMap[entity.SoulID]);
                return entity;
            });
        }


        ItemSchema.Record _itemRecord;

        void _setItemRecord(ItemSchema.Record itemRecord)
        {
            if (SoulID != itemRecord.ID) throw new InvalidOperationException();
            _itemRecord = itemRecord;
        }


        // ItemBuilder作成中に直す
        public PossessionParam GetUsingSoulPossessionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Soul,
                ID = SoulID,
                Num = num
            };
        }

        // ItemBuilder作成中に直す
        public PossessionParam GetCharacterPosssesionParam()
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Character,
                ID = CharacterID,
                Num = 1
            };
        }

        // ItemBuilder作成中に直す
        public PossessionParam GetUsingGoldPosssesionParam(uint gold)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Status,
                ID = (uint)PossessionStatusID.Gold,
                Num = gold
            };
        }

        public IEnumerable<PossessionParam> GetAllPossessableParams(uint gold, uint num)
        {
            return new[]
            {
                GetCharacterPosssesionParam(),
                GetUsingSoulPossessionParam(num),
                GetUsingGoldPosssesionParam(gold)
            };
        }

        public Core.WebApi.Response.Soul ToResponseData()
        {
            return new Core.WebApi.Response.Soul
            {
                ID = SoulID,
                TextKey = TextKey,
                CharacterID = CharacterID,
                Rarity = Rarity
            };
        }

        public uint SoulID => _record.ID;
        public uint CharacterID => _record.CharacterID;
        public string TextKey => "Master.Item." + _itemRecord.TextKey;
        public uint Rarity => _itemRecord.Rarity;
    }
}
