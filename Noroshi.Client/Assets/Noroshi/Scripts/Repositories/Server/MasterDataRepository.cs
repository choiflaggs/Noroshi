using System.Collections.Generic;
using UniRx;
//using MsgPack.Serialization;
using Noroshi.Security;
using Noroshi.WebApi;

namespace Noroshi.Repositories.Server
{
    public class MasterDataRepository<T>
    {
        static private T[] _allDatas;

        private Dictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();

                headers["SessionID"] = WebApiRequester.SessionID;
                return headers;
                //Ž©•ª‚ÌƒAƒvƒŠ‚ÌID‚ð“ü‚ê‚Ä‚Ë
            }
        }

        public MasterDataRepository()
        {
            //_loatByte(_url()).Select(t => UnpackBytes(t)).Subscribe();
            _loatByte(_url()).Select(t => UnpackJson(t)).Subscribe();
        }


        public IObservable<T[]> LoadAll()
        {
            return _load(_url()).Do(ts => _allDatas = ts);
        }

        public virtual IObservable<T> Get(uint id)
        {
            return null;
        }
        public virtual IObservable<T[]> GetMulti(uint[] ids)
        {
            return null;
        }


        protected IObservable<T[]> _load(string url)
        {
            //return _allDatas != null ? Observable.Return(_allDatas) : _loatByte(url).Select(t => UnpackBytes(t));
            return _allDatas != null ? Observable.Return(_allDatas) : _loatByte(url).Select(t => UnpackJson(t));
        }

        IObservable<byte[]> _loatByte(string url)
        {
            return ObservableWWW.GetAndGetBytes(url, Headers);
        }

//        protected virtual T[] UnpackBytes(byte[] bytes)
//        {
//            var stream = new MemoryStream(Cryption.Dencrypt(bytes));
//            var serializer = MessagePackSerializer.Get<T[]>();
//            _allDatas = serializer.Unpack(stream);
//            return _allDatas;
//        }

        protected virtual T[] UnpackJson(byte[] bytes)
        {
            if (bytes.Length <= 0) return null;
            LitJson.JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(System.Convert.ToDouble(obj)));
            LitJson.JsonMapper.RegisterImporter<double, float>(System.Convert.ToSingle);
            var text = System.Text.Encoding.UTF8.GetString(Cryption.Dencrypt(bytes));
            _allDatas = LitJson.JsonMapper.ToObject<T[]>(text);
            return _allDatas;
        }

        protected virtual string _url()
        {
            return GlobalContainer.Config.WebApiHost + "/api/";

        }
    }
}