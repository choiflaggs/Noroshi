using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Daos.Rdb.Story;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStoryEpisodeSchema;

namespace Noroshi.Server.Entity.Story
{
    public class PlayerStoryEpisodeEntity : AbstractDaoWrapperEntity<PlayerStoryEpisodeEntity, PlayerStoryEpisodeDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerStoryEpisodeEntity Get(uint playerId)
        {
            var entity = _instantiate((new PlayerStoryEpisodeDao()).CreateOrSelect(playerId));
            return entity;
        }

        public static PlayerStoryEpisodeEntity ChangeLastEpisode(uint playerId, uint episodeId)
        {
            var entity = _instantiate((new PlayerStoryEpisodeDao()).CreateOrSelect(playerId));
            entity.ChangeEpisodeID(episodeId);
            return entity;
        }

        public Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse ToResponseData()
        {
            var stageData = StoryStageEntity.ReadAndBuildByEpisodeID(EpisodeID);
            var stageQueryList = new List<PlayerStoryStageSchema.PrimaryKey>();
            stageData.ToList().ForEach(d => stageQueryList.Add(new PlayerStoryStageSchema.PrimaryKey { PlayerID = PlayerID, StageID = d.ID }));
            if (!stageQueryList.Any())
            {
                var chapterFirst = StoryChapterEntity.ReadAndBuildAll().OrderBy(c => c.ID).FirstOrDefault();
                return new Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse
                {
                    StageID = 0,
                    ChapterID = chapterFirst.ID,
                    EpisodeID = 0
                };
            }
            var playerStageData = PlayerStoryStageEntity.ReadAndBuildMulti(stageQueryList).OrderByDescending(d => d.StageID).FirstOrDefault();
            if (playerStageData == null) {
                var chapterFirst = StoryChapterEntity.ReadAndBuildAll().OrderBy(c => c.ID).FirstOrDefault();
                return new Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse
                {
                    StageID = 0,
                    ChapterID = chapterFirst.ID,
                    EpisodeID = 0
                };
            }

            if (playerStageData.StageID == stageData.OrderByDescending(d => d.ID).FirstOrDefault().ID && playerStageData.Rank > 0)
            {
                var chapterFirst = StoryChapterEntity.ReadAndBuildAll().OrderBy(c => c.ID).FirstOrDefault();
                return new Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse
                {
                    StageID = 0,
                    ChapterID = chapterFirst.ID,
                    EpisodeID = 0
                };
            }


            var episodeData = StoryEpisodeEntity.ReadAndBuild(new StoryEpisodeSchema.PrimaryKey {ID = EpisodeID});
            return new Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse
            {
                StageID = playerStageData.StageID,
                ChapterID = episodeData.ChapterID,
                EpisodeID = EpisodeID
            };
        }


        public uint PlayerID => _record.PlayerID;

        public uint EpisodeID => _record.EpisodeID;

        public void ChangeEpisodeID(uint episodeId)
        {
            var record = _record.Clone() as Schema.Record;
            if (record == null) {
                throw new InvalidOperationException();
            }
            record.EpisodeID = episodeId;
            _changeLocalRecord(record);
        }
    }
}