using Noroshi.WebApi;
using NoroshiDebug.Datas.Request;
using UniRx;

namespace NoroshiDebug.TimeDebug
{
    public class TimeDebugRepository
    {
        private WebApiRequester _webApiRequester;

        public IObservable<Noroshi.Core.WebApi.Response.Debug.TimeDebug> Get()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Noroshi.Core.WebApi.Response.Debug.TimeDebug>(_url() + "Get");
        }

        public IObservable<Noroshi.Core.WebApi.Response.Debug.TimeDebug> ChangeTime(int year, int month, int day, int hour, int minute, int second)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new TimeDebugChangeTimeRequest { Year = year, Month = month, Day = day, Hour = hour, Minute = minute, Second = second };
            return _webApiRequester.Post<TimeDebugChangeTimeRequest, Noroshi.Core.WebApi.Response.Debug.TimeDebug>(_url() + "ChangeTime", sendData);
        }

        protected string _url()
        {
            return "TimeDebug/";
        }
    }
}
