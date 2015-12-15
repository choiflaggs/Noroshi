using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class CharacterEffectRepository : MasterDataRepository<CharacterEffect>
    {

        public override IObservable<CharacterEffect> Get(uint id)
        {
            return LoadAll().Select(ts => ts.Where(t => t.ID == id).FirstOrDefault());
        }
        public override IObservable<CharacterEffect[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.ID);
                return ids.Select(id => map[id]).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "CharacterEffect/MasterData";
        }
    }
}
