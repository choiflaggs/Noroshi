using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Story;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Daos.Rdb.Story;
using Noroshi.Server.Services.Utility;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStoryStageSchema;

namespace Noroshi.Server.Entity.Story
{
    public class PlayerStoryStageEntity : AbstractDaoWrapperEntity<PlayerStoryStageEntity, PlayerStoryStageDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerStoryStageEntity Get(uint playerId, uint stageId)
        {
            var stageData = StoryStageEntity.ReadAndBuild(stageId);
            if (stageData == null)
            {
                throw new InvalidOperationException();
            }
            var entity = _instantiate((new PlayerStoryStageDao()).CreateOrSelect(playerId, stageId));
            return entity;
        }

        public void ResetTimeCheck(uint stageId)
        {
            var updateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0);
            if (UpdatedAt >= updateTime || updateTime >= DateTime.Now) return;
            AddMaxPlayCount();
        }

        public static PlayerStoryStageEntity AddResetCount(uint playerId, uint stageId)
        {
            var entity = _instantiate((new PlayerStoryStageDao()).CreateOrSelect(playerId, stageId));
            entity.AddMaxPlayCount();
            entity.Save();
            return entity;
        }


        public static PlayerStoryStageEntity UsePlayCount(uint playerId, uint stageId)
        {
            var entity = _instantiate((new PlayerStoryStageDao()).CreateOrSelect(playerId, stageId));
            entity.AddPlayCount();
            entity.Save();
            return entity;
        }

        public static PlayerStoryStageEntity ChangedProgress(uint playerId, uint stageId, byte progress)
        {
            var entity = _instantiate((new PlayerStoryStageDao()).CreateOrSelect(playerId, stageId));
            entity.ChangedProgress(progress);
            return entity;
        }


        public uint PlayerID => _record.PlayerID;

        public uint StageID => _record.StageID;

        public byte Rank => _record.Rank;
        public uint PlayCount => _record.PlayCount;
        public uint MaxPlayCount => _record.MaxPlayCount;
        public DateTime UpdatedAt => UnixTime.FromUnixTime(_record.UpdatedAt);

        public Core.WebApi.Response.Story.PlayerStoryStage ToResponseData(Core.WebApi.Response.Story.StoryStage stage)
        {
            var stageData = StoryStageEntity.ReadAndBuild(new StoryStageSchema.PrimaryKey { ID = StageID });
            return new Core.WebApi.Response.Story.PlayerStoryStage
            {
                StageID = StageID,
                Rank = Rank,
                Stage = stage,
                PlayCount = stageData.Type != Enums.StageType.BackStoryStage ? uint.MaxValue : (MaxPlayCount + 1) * Constant.BACK_STAGE_REMAIN_CHANCE - PlayCount
            };
        }
        public static IEnumerable<PlayerStoryStageEntity> ReadAndBuildMultiByPlayerID(uint playerId)
        {
            var dao = new PlayerStoryStageDao();
            return _instantiate(dao.ReadByPlayerID(playerId));
        }

        public static PlayerStoryStageEntity ReadAndBuildByPlayerIDAndStageID(uint playerId, uint stageId)
        {
            var dao = new PlayerStoryStageDao();
            var playerStageData = dao.ReadByPlayerIDAndStageID(playerId, stageId);
            return _instantiate(playerStageData);
        }

        public void AddMaxPlayCount(uint count = 2)
        {
            var record = _record.Clone() as Schema.Record;
            if (record == null)
            {
                throw new InvalidOperationException();
            }
            record.MaxPlayCount =  PlayCount + count;
            record.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }

        public void AddPlayCount(uint count = 1)
        {
            var record = _record.Clone() as Schema.Record;
            if (record == null) {
                throw new InvalidOperationException();
            }
            record.PlayCount += count;
            record.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);

        }


        public void ChangedProgress(byte rank)
        {
            if (rank <= 0 || rank > 3) {
                throw new InvalidOperationException();
            }
            var record = _record.Clone() as Schema.Record;
            if (record == null) {
                throw new InvalidOperationException();
            }
            record.Rank = rank;
            record.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }

    }
}