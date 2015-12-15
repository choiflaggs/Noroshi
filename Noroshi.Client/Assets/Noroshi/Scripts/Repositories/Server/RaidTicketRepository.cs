using Noroshi.Core.WebApi.Response;
using UniRx;
using UniLinq;


namespace Noroshi.Repositories.Server
{
    public class RaidTicketRepository : MasterDataRepository<RaidTicket>
    {

        public override IObservable<RaidTicket> Get(uint ID)
        {
            return LoadAll().Select(ts => ts.FirstOrDefault(t => t.ID == ID));
        }
        protected override string _url()
        {
            return base._url() + "RaidTicket/MasterData";
        }
    }
}