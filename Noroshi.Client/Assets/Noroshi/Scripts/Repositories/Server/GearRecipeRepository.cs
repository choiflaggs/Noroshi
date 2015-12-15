using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class GearRecipeRepository : MasterDataRepository<GearRecipe>
    {

        public override IObservable<GearRecipe> Get(uint id)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.CraftItemID == id));
        }
        public override IObservable<GearRecipe[]> GetMulti(uint[] ids)
        {
            return LoadAll().Select(ts =>
            {
                var map = ts.ToDictionary(t => t.CraftItemID);
                return ids.Select(id => map[id]).ToArray();
            });
        }



        protected override string _url()
        {
            return base._url() + "GearRecipe/MasterData";
        }

        public IObservable<GearRecipe[]> GetRecipe(uint craftItemID)
        {
            return LoadAll().Select(ts => ts.Where(t => t.CraftItemID == craftItemID).ToArray());
        }
    }

}
