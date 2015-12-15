using Noroshi.Core.WebApi.Response.Master;
using UniRx;
using UniLinq;

namespace Noroshi.Repositories.Server
{
    public class LevelMasterRepository
    {
        Core.WebApi.Response.Master.LevelMasterResponse _response;

        public IObservable<PlayerLevel> GetPlayerLevel(ushort level)
        {
            return _load().Select(res => res.PlayerLevels.Where(cl => cl.Level == level).FirstOrDefault());
        }
        public IObservable<CharacterLevel> GetCharacterLevel(ushort level)
        {
            return _load().Select(res => res.CharacterLevels.Where(cl => cl.Level == level).FirstOrDefault());
        }



        IObservable<LevelMasterResponse> _load()
        {
            return _response != null ? Observable.Return(_response) : (new WebApi.WebApiRequester()).Request<LevelMasterResponse>("Master/LevelMaster").Do(res => _response = res);
        }
    }
}
