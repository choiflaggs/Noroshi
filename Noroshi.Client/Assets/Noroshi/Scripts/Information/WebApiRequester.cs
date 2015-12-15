using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Information;
using Noroshi.Core.Game.Information;

namespace Noroshi.Information
{
    public class WebApiRequester
    {
        /// <summary>
        /// お知らせ一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("Information/List");
        }
        /// <summary>
        /// お知らせを既読にする。
        /// </summary>
        /// <param name="informationCategories">既読にするお知らせカテゴリ</param>
        public static IObservable<ReadResponse> Read(IEnumerable<InformationCategory> informationCategories)
        {
            // サーバー側が byte[] をパラメータとして扱えないので ushort[] でお茶を濁す。
            var requestParams = new ReadRequest
            {
                informationCategories = informationCategories.Select(ic => (ushort)ic).ToArray(),
            };
            return _getWebApiRequester().Post<ReadRequest, ReadResponse>("Information/Read", requestParams);
        }
        class ReadRequest
        {
            public ushort[] informationCategories { get; set; }
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
