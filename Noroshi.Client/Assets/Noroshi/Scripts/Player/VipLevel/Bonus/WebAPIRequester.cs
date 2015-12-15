using Noroshi.Core.WebApi.Response;
using UniRx;

namespace Noroshi.Player.VipLevel.Bonus
{
    public class WebAPIRequester
    {
        protected string _url()
        {
            return "PlayerVipLevelBonus/";
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }

        public IObservable<PlayerVipLevelBonus[]> MasterData()
        {
            return _getWebApiRequester().Post<PlayerVipLevelBonus[]>(_url() + "MasterData");
        }
    }
}
