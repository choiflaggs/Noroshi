using Noroshi.Core.WebApi.Response.Character;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class AttributeRepository : MasterDataRepository<Attribute>
    {

        public override IObservable<Attribute> Get(uint id)
        {
            return LoadAll().Select(ts => ts.Where(t => t.ID == id).FirstOrDefault());
        }
        public override IObservable<Attribute[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.ID);
                return ids.Select(id => map[id]).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "Attribute/MasterData";
        }
    }
}
