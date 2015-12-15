using UniRx;
using Noroshi.Core.WebApi.Response.Trial;

namespace Noroshi.Trial
{
    public class WebApiRequester
    {
        /// <summary>
        /// 試練一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("Trial/List");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
