using System;
using UniLinq;
using LitJson;
using UnityEngine;
using UniRx;
using Noroshi.Datas;

namespace Noroshi.Repositories
{
	public class MasterDataRepository<T> : IMasterDataRepository<T> where T : MasterData
	{
		T[] _allDatas = null;

		public IObservable<T[]> LoadAll()
		{
			return _load(_filePath()).Do(ts => _allDatas = ts);
		}

        public IObservable<T> Get(uint id)
		{
			return LoadAll().Select(ts => ts.Where(t => t.ID == id).FirstOrDefault());
		}
        public IObservable<T[]> GetMulti(uint[] ids)
		{
			return LoadAll().Select(ts => {
				var map = ts.ToDictionary(t => t.ID);
				return ids.Select(id => map[id]).ToArray();
			});
		}


		protected IObservable<T[]> _load(string filePath)
		{
			LitJson.JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
			return _loatText(filePath).Select(t => JsonMapper.ToObject<T[]>(t));
		}
		protected IObservable<string> _loatText(string filePath)
		{
			var stageTextAsset = Resources.Load(filePath) as TextAsset;
			return Observable.Return<string>(stageTextAsset.text);
		}

		protected virtual string _filePath() { return ""; }
	}
}
