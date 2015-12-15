using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Services.Player
{
    public class PlayerStoryEpisodeService
    {
        public static Core.WebApi.Response.Story.PlayerStoryEpisode[] ChangeLastEpisode(uint episodeId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            ContextContainer.NoroshiTransaction(tx =>
            {
                var entity = PlayerStoryEpisodeEntity.ChangeLastEpisode(playerId, episodeId);
                var stageData = StoryStageEntity.ReadAndBuildByEpisodeID(episodeId).OrderBy(s => s.ID).FirstOrDefault();
                if (entity == null)
                {
                    return false;
                }
                var playerStage = PlayerStoryStageEntity.Get(playerId, stageData.ID);
                playerStage.Save();
                var result = entity.Save();
                tx.Commit();
                return result;
            });
            return GetAll();
        }

        public static Core.WebApi.Response.Story.PlayerLastPlayStoryStageResponse GetLastStage()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerStoryEpisodeEntity.Get(playerId).ToResponseData();
        }

        public static Core.WebApi.Response.Story.PlayerStoryEpisode[] GetAll()
        {
            var playerStageData = PlayerStoryStageService.GetAll();
            var stageIds = playerStageData.Select(r => new StoryStageSchema.PrimaryKey {ID = r.StageID});
            var returnList = new List<Core.WebApi.Response.Story.PlayerStoryEpisode>();
            if (!stageIds.Any()) return returnList.ToArray();
            var stageDatas =  StoryStageEntity.ReadAndBuildMulti(stageIds);
            var episodeStageData = StoryStageEntity.ReadAndBuildAll();
            var episodeIds = stageDatas.Select(r => r.EpisodeID).Distinct();
            episodeIds.ToList().ForEach(r =>
            {
                var tmpData = new Core.WebApi.Response.Story.PlayerStoryEpisode
                {
                    EpisodeID = r
                };
                var tmpStageData = episodeStageData.Where(s => s.EpisodeID == r);
                var tmpStageIds = tmpStageData.Select(t => t.ID);
                tmpData.PlayerStageList = playerStageData.Where(s => tmpStageIds.Any(key => s.StageID == key)).ToArray();
                var lastStage = tmpStageData.OrderByDescending(b => b.ID).FirstOrDefault();
                var lastPlayStage = tmpData.PlayerStageList.OrderByDescending(b => b.StageID).FirstOrDefault();
                tmpData.IsClearEpisode = (lastStage.ID == lastPlayStage.StageID && lastPlayStage.Rank > 0);
                returnList.Add(tmpData);
            });
            return returnList.ToArray();
        }
    }
}