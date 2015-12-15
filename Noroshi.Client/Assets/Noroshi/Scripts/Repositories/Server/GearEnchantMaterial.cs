using System.Collections.Generic;
using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class GearEnchantMaterialRepository : MasterDataRepository<GearEnchantMaterial>
    {

        public override IObservable<GearEnchantMaterial> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }

        protected override string _url()
        {
            return base._url() + "GearEnchantMaterial/MasterData";
        }
    }
}
