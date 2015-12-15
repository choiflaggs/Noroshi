using Noroshi.Core.WebApi.Response;
using Noroshi.Core.WebApi.Response.Story;
using UniLinq;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class StoryStageRepository
    {
        StoryStage[] _stages;

        public IObservable<StoryStage> Get(uint stageId)
        {
            return GetAll().Select(ts => ts.FirstOrDefault(t => t.ID == stageId));
        }

        public IObservable<StoryStage[]> GetAll()
        {
            return _stages != null ? Observable.Return(_stages) : Story.WebAPIRequester.MasterData().Select(res =>
            {
                _stages = res.Stages;
                return _stages;
            });
        }
    }
}
