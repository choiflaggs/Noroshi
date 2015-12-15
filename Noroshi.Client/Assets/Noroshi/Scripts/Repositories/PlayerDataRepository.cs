using System.Collections.Generic;
using UniLinq;
using LitJson;
using UnityEngine;
using UniRx;
using Noroshi.Datas;

namespace Noroshi.Repositories
{
	public class PlayerDataRepository<T> where T : PlayerData
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
			return LoadAll().Select(ts => ts.Where(t => ids.Contains(t.ID)).ToArray());
		}


		protected IObservable<T[]> _load(string filePath)
		{
			return _loatText(filePath).Select(t => JsonMapper.ToObject<T[]>(t));
		}
		IObservable<string> _loatText(string filePath)
		{
			var stageTextAsset = Resources.Load(filePath) as TextAsset;
			return Observable.Return<string>(stageTextAsset.text);
		}

		protected virtual string _filePath() { return ""; }
	}
}
