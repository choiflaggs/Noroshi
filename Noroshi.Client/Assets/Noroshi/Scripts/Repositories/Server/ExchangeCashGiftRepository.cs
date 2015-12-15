using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class ExchangeCashGiftRepository : MasterDataRepository<ExchangeCashGift>
    {

        public override IObservable<ExchangeCashGift> Get(uint ID)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == ID));
        }
        protected override string _url()
        {
            return base._url() + "ExchangeCashGift/MasterData";
        }
    }
}