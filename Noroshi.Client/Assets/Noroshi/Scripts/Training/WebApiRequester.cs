using UniRx;
using Noroshi.Core.WebApi.Response.Training;

namespace Noroshi.Training
{
    public class WebApiRequester
    {
        /// <summary>
        /// 修行一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("Training/List");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
