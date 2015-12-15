using System;
using System.Collections.Generic;

namespace Noroshi
{
	public class Container
	{
		Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();
		Dictionary<Type, object>       _instances = new Dictionary<Type, object>();
		
		public void SetFactory<T>(Func<T> factory)
		{
			if (_factories.ContainsKey(typeof(T)))
			{
				_factories.Remove(typeof(T));
			}
			_factories.Add(typeof(T), () => (object)factory());
		}

		public bool Contains<T>()
        {
            return _instances.ContainsKey(typeof(T));
        }
		
		public T Get<T>()
		{
			if (!_instances.ContainsKey(typeof(T)))
			{
				_instances.Add(typeof(T), _factories[typeof(T)].Invoke());
			}
			return (T)_instances[typeof(T)];
		}

        public void Clear()
        {
            _factories.Clear();
            _instances.Clear();
        }
	}
}
