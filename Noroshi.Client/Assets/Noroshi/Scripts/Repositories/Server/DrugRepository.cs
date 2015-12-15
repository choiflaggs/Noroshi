using System.Collections.Generic;
using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class DrugRepository : MasterDataRepository<Drug>
    {

        public override IObservable<Drug> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }

        protected override string _url()
        {
            return base._url() + "Drug/MasterData";
        }
    }
}
