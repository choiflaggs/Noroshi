using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Services.Player
{
    public class PlayerStoryChapterService
    {
        public static PlayerStoryChapter[] GetAll()
        {
            var playerEpisodeData = PlayerStoryEpisodeService.GetAll();
            var episodeIds = new List<StoryEpisodeSchema.PrimaryKey>();
            var returnList = new List<PlayerStoryChapter>();
            if (!episodeIds.Any()) return returnList.ToArray();
            playerEpisodeData.ToList().ForEach(data =>
            {
                var tmpData = new StoryEpisodeSchema.PrimaryKey {ID = data.EpisodeID};
                episodeIds.Add(tmpData);
            });
            var episodeDatas = StoryEpisodeEntity.ReadAndBuildMulti(episodeIds);
            var chapterIds = new List<StoryChapterSchema.PrimaryKey>();
            episodeDatas.ToList().ForEach(data =>
            {
                var tmpData = new StoryChapterSchema.PrimaryKey {ID = data.ChapterID };
                chapterIds.Add(tmpData);

            }
            );
            var chapterData = StoryChapterEntity.ReadAndBuildMulti(chapterIds);

            chapterData.ToList().ForEach(data =>
            {
                var tmpData = new PlayerStoryChapter {ChapterID = data.ID};
                var tmpEpisodeIds = episodeDatas.Where(s => s.ChapterID == data.ID).Select(t => t.ID);
                tmpData.PlayerEpisodeList = playerEpisodeData.Where(s => tmpEpisodeIds.Any(key => s.EpisodeID == key)).ToArray();
                returnList.Add(tmpData);

            });
            return returnList.ToArray();
        }

        public static StoryChapterAndStoryEpisodeResponse[] GetAllAndMaster()
        {
            var Chapterdatas = StoryChapterEntity.ReadAndBuildAll().Select(c => c.ToResponseData());
            var Episodedatas = StoryEpisodeEntity.ReadAndBuildAll().Select(e => e.ToResponseData());
            var playerEpisodeDatas = PlayerStoryEpisodeService.GetAll().ToDictionary(e => e.EpisodeID);
            var returnData = new List<StoryChapterAndStoryEpisodeResponse>();
            Chapterdatas.ToList().ForEach(chapter =>
            {
                var tmpEpisodeDatas = Episodedatas.Where(episode => episode.ChapterID == chapter.ID);
                var playerStageAndEpisode = new List<PlayerStoryStageAndPlayerStoryEpisodeResponse>();
                tmpEpisodeDatas.ToList().ForEach(episode =>
                {
                    playerStageAndEpisode.Add(new PlayerStoryStageAndPlayerStoryEpisodeResponse { PlayerEpisode = playerEpisodeDatas.ContainsKey(episode.ID) ? playerEpisodeDatas[episode.ID] : null, Episode = episode});
                });
                returnData.Add(new StoryChapterAndStoryEpisodeResponse {Chapter = chapter, Episodes = playerStageAndEpisode.ToArray()});
            });
            return returnData.ToArray();
        }

    }
}