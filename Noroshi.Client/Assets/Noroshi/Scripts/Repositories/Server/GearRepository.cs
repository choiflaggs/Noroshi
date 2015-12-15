using System.Collections.Generic;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class GearRepository : MasterDataRepository<Core.WebApi.Response.Gear>
    {

        public override IObservable<Core.WebApi.Response.Gear> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }
        public override IObservable<Core.WebApi.Response.Gear[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = new Dictionary<uint, Core.WebApi.Response.Gear>();
                foreach (var t in ts)
                {
                    if (!map.ContainsKey(t.ID))
                    {
                        map.Add(t.ID, t);
                    }
                }
                return ids.Select(id => map.ContainsKey(id) ? map[id] : null).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "Gear/MasterData";
        }
    }
}
