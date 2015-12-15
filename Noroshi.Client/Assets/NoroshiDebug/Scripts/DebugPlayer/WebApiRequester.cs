using UniRx;
using Noroshi.Core.WebApi.Response.Debug;

namespace NoroshiDebug.DebugPlayer
{
    public class WebApiRequester
    {
        /// プレイヤー情報リセット実行。レスポンスが帰ってきた後はログインシーンへ遷移してください。
        public static IObservable<PlayerDebugResponse> Reset()
        {
            return _getWebApiRequester().Post<PlayerDebugResponse>("PlayerDebug/Reset");
        }
        /// プレイヤー情報入れ替え実行。レスポンスが帰ってきた後はログインシーンへ遷移してください。
        public static IObservable<PlayerDebugResponse> Swap(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, PlayerDebugResponse>("PlayerDebug/Swap", request);
        }

        class TargetPlayerIDRequest
        {
            public uint TargetPlayerID { get; set; }
        }
        static Noroshi.WebApi.WebApiRequester _getWebApiRequester()
        {
            return new Noroshi.WebApi.WebApiRequester();
        }
    }
}
