using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerRaidTicketRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "RaidTicket/";
        }

        public IObservable<Core.WebApi.Response.PlayerRaidTicket[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerRaidTicket[]>(_url() + "GetAll");

        }
    }
}
