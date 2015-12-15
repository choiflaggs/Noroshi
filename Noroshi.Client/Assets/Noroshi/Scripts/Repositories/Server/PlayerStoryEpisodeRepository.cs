using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Datas.Request;
using Noroshi.WebApi;
using UniLinq;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerStoryEpisodeRepository : PlayerDataRepository<PlayerStoryEpisode>
    {
        public IObservable<PlayerLastPlayStoryStageResponse> GetLastStage()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerLastPlayStoryStageResponse>(_url() + "GetLastStoryStage");
        }

        public IObservable<PlayerStoryEpisode[]> ChangeLastEpisode(uint episodeId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var query = new PlayerEpisodeChangeEpisodeRequest {EpisodeID = episodeId};
            return _webApiRequester.Post<PlayerEpisodeChangeEpisodeRequest, PlayerStoryEpisode[]>(_url() + "ChangeLastStoryEpisode", query);
        }


        public override IObservable<PlayerStoryEpisode[]> GetAll()
        {
            return GetData();
        }

        public override IObservable<PlayerStoryEpisode> Get(uint id)
        {
            return GetData().Select(e => e.ToList().FirstOrDefault(f => f.EpisodeID == id));
        }



        protected override IObservable<PlayerStoryEpisode[]> GetData()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerStoryEpisode[]>(_url() + "GetAll");
        }

        protected string _url()
        {
            return "PlayerStoryEpisode/";
        }
    }
}