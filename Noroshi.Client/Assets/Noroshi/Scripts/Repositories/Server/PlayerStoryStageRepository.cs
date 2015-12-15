using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Datas.Request;
using Noroshi.WebApi;
using UniLinq;
using UniRx;

namespace Noroshi.Repositories.Server
{

    public class PlayerStoryStageRepository : PlayerDataRepository<PlayerStoryStage>
    {
        protected override string _url()
        {
            return "PlayerStoryStage/";
        }

        public override IObservable<PlayerStoryStage> Get(uint id)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var query = new PlayerStatusIDRequest {ID = id};
            return _webApiRequester.Post<PlayerStatusIDRequest, PlayerStoryStage>(_url() + "Get", query);
        }

        public IObservable<PlayerStoryStage[]> GetByEpisodeID(uint episodeId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var query = new PlayerStageEpisodeIdRequest { EpisodeID = episodeId };
            return _webApiRequester.Post<PlayerStageEpisodeIdRequest, PlayerStoryStage[]>(_url() + "GetByEpisodeID", query);
        }

        protected override IObservable<PlayerStoryStage[]> GetData()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerStoryStage[]>(_url() + "GetAll").Do(
                data => _data = data.ToList()
                );
        }
    }
}