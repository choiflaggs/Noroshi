using UniRx;
using Noroshi.Core.WebApi.Response.LoginBonus;

namespace Noroshi.LoginBonus
{
    public class WebApiRequester
    {
        /// <summary>
        /// ログインボーナス一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("LoginBonus/List");
        }
        /// <summary>
        /// 報酬を受け取る。
        /// </summary>
        /// <param name="loginBonusId">ログインボーナス ID</param>
        /// <param name="threshold">敷居値（何日目のログインボーナスか）</param>
        public static IObservable<ReceiveRewardResponse> ReceiveReward(uint loginBonusId, byte threshold)
        {
            var requestParams = new ReceiveRewardRequest
            {
                LoginBonusID = loginBonusId,
                Threshold = threshold,
            };
            return _getWebApiRequester().Post<ReceiveRewardRequest, ReceiveRewardResponse>("LoginBonus/ReceiveReward", requestParams);
        }
        class ReceiveRewardRequest
        {
            public uint LoginBonusID { get; set; }
            public byte Threshold { get; set; }
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
