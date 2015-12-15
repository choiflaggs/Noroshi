using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class SoulRepository : MasterDataRepository<Soul>
    {
        public IObservable<Soul> GetByCharacterId(uint characterID)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.CharacterID == characterID));
        }
        protected override string _url()
        {
            return base._url() + "Soul/MasterData";
        }
    }
}