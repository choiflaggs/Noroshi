using UniLinq;
using Noroshi.WebApi;
using UniRx;
using System.Collections.Generic;

namespace Noroshi.Repositories.Server
{
    public class PlayerDataRepository<T>
    {
        protected List<T> _data;
        protected WebApiRequester _webApiRequester;

        public virtual IObservable<T> Get(uint id)
        {
            return GetAll().Select(ts => ts.FirstOrDefault());
        }

        public virtual IObservable<T[]> GetAll()
        {
            return _data != null
                ? Observable.Return(_data.ToArray())
                : GetData().Select(_ => _data.ToArray());
            
        }

        protected virtual IObservable<T[]> GetData()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<T[]>(_url() + "Get").Do(
                data => _data = data.ToList()
                );
        }

        protected virtual string _url()
        {
            return "";
        }
    }
}