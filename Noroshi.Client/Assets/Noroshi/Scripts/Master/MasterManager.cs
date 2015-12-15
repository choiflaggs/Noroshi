using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.WebApi;
using Noroshi.Core.WebApi.Response.Master;

namespace Noroshi.Master
{
    public class MasterManager
    {
        public readonly TextMaster TextMaster = new TextMaster();
        public readonly LevelMaster LevelMaster = new LevelMaster();
        public readonly SoundMaster SoundMaster = new SoundMaster();

        public IObservable<MasterManager> LoadAll(IWebApiRequester webApiRequester)
        {
            var requests = new []
            {
                TextMaster.Load(webApiRequester).Select(_ => this),
                LevelMaster.Load(webApiRequester).Select(_ => this),
                SoundMaster.Load(webApiRequester).Select(_ => this),
            };
            return requests.WhenAll().Select(_ => this);
        }
    }
    public abstract class AbstractMaster<T>
        where T : class
    {
        protected T _response;
        public IObservable<T> Load(IWebApiRequester webApiRequester)
        {
            return _response != null ? Observable.Return(_response) : webApiRequester.Request<T>("Master/" + GetType().Name).Do(response => _response = response);
        }
    }
    public class TextMaster : AbstractMaster<TextMasterResponse>
    {
        public DynamicText[] GetAllDynamicTexts()
        {
            return _response.DynamicTexts;
        }
    }
    public class LevelMaster : AbstractMaster<LevelMasterResponse>
    {
    }
    public class SoundMaster : AbstractMaster<SoundMasterResponse>
    {
        Dictionary<uint, Sound> _cache;
        public Sound Get(uint id)
        {
            _setCacheIfNeed();
            return _cache[id];
        }
        void _setCacheIfNeed()
        {
            if (_cache == null) _cache = _response.Sounds.ToDictionary(s => s.ID);
        }
    }
}
