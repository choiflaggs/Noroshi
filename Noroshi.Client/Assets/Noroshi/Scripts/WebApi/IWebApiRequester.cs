using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Noroshi.WebApi
{
    public interface IWebApiRequester
    {
        /// Web API へリクエストを送るメソッド。パラメータなし版。
        IObservable<RES> Request<RES>(string path);
        /// Web API へリクエストを送るメソッド。POST版。
        IObservable<RES> Post<REQ, RES>(string path, REQ requestParam);
        /// Web API へリクエストを送るメソッド。暗号化付きPOST版。
        IObservable<RES> PostWithCrypt<REQ, RES>(string path, REQ requestParam);
    }
}