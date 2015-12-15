using System.Collections.Generic;
using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class GearPieceRepository : MasterDataRepository<GearPiece>
    {

        public override IObservable<GearPiece> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }
        public override IObservable<GearPiece[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = new Dictionary<uint, GearPiece>();
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
            return base._url() + "GearPiece/MasterData";
        }
    }
}
