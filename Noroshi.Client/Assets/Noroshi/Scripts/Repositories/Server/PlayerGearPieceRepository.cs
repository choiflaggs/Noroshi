using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerGearPieceRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "GearPiece/";
        }

        public IObservable<Core.WebApi.Response.PlayerGearPiece[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerGearPiece[]>(_url() + "GetAll");

        }
    }
}
