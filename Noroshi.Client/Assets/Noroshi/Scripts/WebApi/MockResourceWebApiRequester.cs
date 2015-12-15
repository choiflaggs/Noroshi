using System;
using System.Collections.Generic;
using UniRx;
using LitJson;
using UnityEngine;

namespace Noroshi.WebApi
{
    public class MockResourceWebApiRequester : IWebApiRequester
    {
        const string BASE_DIR = "Debug/Mock/WebApiResponse";

        public IObservable<RES> Request<RES>(string path)
        {
            return _load<RES>(BASE_DIR + path);
        }
        public IObservable<RES> Post<REQ, RES>(string path, REQ requestParam)
        {
            return _load<RES>(BASE_DIR + path);
        }
        public IObservable<RES> PostWithCrypt<REQ, RES>(string path, REQ requestParam)
        {
            return _load<RES>(BASE_DIR + path);
        }
        protected IObservable<T> _load<T>(string filePath)
        {
            GlobalContainer.Logger.Debug(filePath);
            LitJson.JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
            return _loatText(filePath).Select(t => JsonMapper.ToObject<T>(t));
        }
        protected IObservable<string> _loatText(string filePath)
        {
            var stageTextAsset = Resources.Load(filePath) as TextAsset;
            return Observable.Return<string>(stageTextAsset.text);
        }
    }
}