using System.Collections.Generic;
using Noroshi.Core.WebApi.Response;
using Noroshi.Login;
//using MsgPack.Serialization;
using Noroshi.Security;
using UnityEngine;
using UniRx;

namespace Noroshi.WebApi
{
    public class WebApiRequester : IWebApiRequester
    {
        string _baseUrl;
        public static string SessionID { get; private set; }
        public static void ResetSessionID()
        {
            SessionID = null;
        }

        public static IObservable<string> Login()
        {
            var form = new WWWForm();
            form.AddField("udid", SystemInfo.deviceUniqueIdentifier);
            return SessionID != null ? Observable.Return(SessionID) : ObservableWWW.PostAndGetBytes(GlobalContainer.Config.WebApiHost + "/api/Login/Get", form).Select(t => unpackJson<SessionData>(t).SessionID).Do(id =>
            {
                SessionID = id;
            });

        }

        private Dictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();

                headers["SessionID"] = SessionID;
                return headers;
                //自分のアプリのIDを入れてね
            }
        }
        public WebApiRequester()
        {
            _baseUrl = GlobalContainer.Config.WebApiHost + "/api/";
        }
        public WebApiRequester(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IObservable<RES> Request<RES>(string path)
        {
            var url = _baseUrl + path;

            GlobalContainer.Logger.Debug(url);
            return ObservableWWW.GetAndGetBytes(url, Headers).Select(t => unpackJson<RES>(t));
        }

        public IObservable<RES> Request<REQ, RES>(string path, REQ requestParam)
        {

            var strBuilder = new System.Text.StringBuilder();
            var propertyInfos = typeof(REQ).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetGetMethod().Invoke(requestParam, null);
                // TODO : ちゃんと実装
                if (propertyInfo.PropertyType == typeof(uint[]))
                {
                    foreach (var v in (uint[])value)
                    {
                        strBuilder.Append(propertyInfo.Name + "=" + v.ToString() + "&");
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                }
                else
                {
                    strBuilder.Append(propertyInfo.Name + "=" + value.ToString());
                }
            }

            var url = _baseUrl + path + "?" + strBuilder.ToString();
            GlobalContainer.Logger.Debug(url);
            return ObservableWWW.GetAndGetBytes(url, Headers).Select(t => unpackJson<RES>(t));
        }

        public IObservable<RES> Post<REQ, RES>(string path, REQ requestParam)
        {
            var form = new WWWForm();
            // 動的にパラメータを構築してみる。
            var propertyInfos = typeof(REQ).GetProperties();
            foreach (var propertyInfo in propertyInfos) {
                var value = propertyInfo.GetGetMethod().Invoke(requestParam, null);
                // TODO : ちゃんと実装
                if (propertyInfo.PropertyType == typeof(uint[])) {
                    foreach (var v in (uint[])value) {
                        form.AddField(propertyInfo.Name, v.ToString());
                    }
                } else {
                    form.AddField(propertyInfo.Name, value != null ? value.ToString() : "null");
                }
            }
            var url = _baseUrl + path;
            GlobalContainer.Logger.Debug(url);
            return ObservableWWW.PostAndGetBytes(url, form, Headers)
                .Select(t => unpackJson<RES>(t));
        }
        public IObservable<RES> Post<RES>(string path)
        {
            var form = new WWWForm();
            form.AddField("_d", 1); // Dummy これがないと動かない
            var url = _baseUrl + path;
            return ObservableWWW.PostAndGetBytes(url, form, Headers).Select(t => unpackJson<RES>(t));
        }
        public IObservable<RES> PostWithCrypt<REQ, RES>(string path, REQ requestParam)
        {
            var url = _baseUrl + path;
            //var stream = new MemoryStream();
            //var serializer = MessagePackSerializer.Get<REQ>();
            //serializer.Pack(stream, requestParam);
            //return ObservableWWW.PostAndGetBytes(url, Cryption.Encrypt(stream.GetBuffer()))
            //.Select(t => unpackBytes<RES>(t));
            var text = LitJson.JsonMapper.ToJson(requestParam);
            return ObservableWWW.PostWWW(url, System.Text.Encoding.UTF8.GetBytes(text), Headers)
                .Select(form => unpackJson<RES>(form.bytes));
        }

        public IObservable<RES> RequestSnedJson<RES>(string path, WWWForm form)
        {
            var url = _baseUrl + path;
            return ObservableWWW.PostAndGetBytes(url, form, Headers).Select(t => unpackJson<RES>(t));
        }

        IObservable<byte[]> _loatByte(string url)
        {
            return ObservableWWW.GetAndGetBytes(url);
        }

        //        T unpackBytes<T>(byte[] bytes)
        //        {
        //            var stream = new MemoryStream(Cryption.Dencrypt(bytes));
        //            var serializer = MessagePackSerializer.Get<T>();
        //            return serializer.Unpack(stream);
        //        }

        private IObservable<string> _loatText(string url)
        {
            return ObservableWWW.Get(url);
        }


        private static T unpackJson<T>(byte[] bytes)
        {
            LitJson.JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(System.Convert.ToDouble(obj)));
            LitJson.JsonMapper.RegisterImporter<double, float>(input => System.Convert.ToSingle(input));
            var text = System.Text.Encoding.UTF8.GetString(Cryption.Dencrypt(bytes));
            return LitJson.JsonMapper.ToObject<T>(text);
        }

    }
}