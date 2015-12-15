using Noroshi.Core.WebApi.Response.Story;
using UniLinq;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class StoryEpisodeRepository : MasterDataRepository<StoryEpisode>
    {

        public override IObservable<StoryEpisode> Get(uint episodeId)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == episodeId));
        }
        public override IObservable<StoryEpisode[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.ID);
                return ids.Select(id => map[id]).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "StoryEpisode/MasterData";
        }
    }
}
