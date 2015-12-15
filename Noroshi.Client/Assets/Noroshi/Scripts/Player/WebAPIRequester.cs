using UniRx;
using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Player
{
    public class WebAPIRequester
    {
		protected string _url()
		{
			return "Player/";
		}

		static WebApi.WebApiRequester _getWebApiRequester()
		{
			return new WebApi.WebApiRequester();
		}

		public IObservable<PlayerServiceResponse> RecoverStamina()
		{
			return _getWebApiRequester().Post<PlayerServiceResponse>(_url() + "RecoverStamina");
		}
		
		public IObservable<PlayerServiceResponse> RecoverBP()
		{
			return _getWebApiRequester().Post<PlayerServiceResponse>(_url() + "RecoverBP");
		}

        public IObservable<PlayerServiceResponse> RecoverGold()
        {
            return _getWebApiRequester().Post<PlayerServiceResponse>(_url() + "RecoverGold");
        }
        public IObservable<PlayerServiceResponse> RecoverActionLevelPoint()
        {
            return _getWebApiRequester().Post<PlayerServiceResponse>(_url() + "RecoverActionLevelPoint");
        }
    }
}
