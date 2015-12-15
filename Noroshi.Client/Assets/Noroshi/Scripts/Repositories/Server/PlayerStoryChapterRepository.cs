using Noroshi.Core.WebApi.Response.Story;
using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerStoryChapterRepository : PlayerDataRepository<PlayerStoryChapter>
    {
        protected virtual IObservable<PlayerStoryChapter[]> GetData()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerStoryChapter[]>(_url() + "GetAll");
        }

        public override IObservable<PlayerStoryChapter[]> GetAll()
        {
            return GetData();
        }

        public IObservable<StoryChapterAndStoryEpisodeResponse[]> GetAllAndMaster()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<StoryChapterAndStoryEpisodeResponse[]>(_url() + "GetAllAndMaster");
        }

        protected string _url()
        {
            return "PlayerStoryChapter/";
        }

    }
}
