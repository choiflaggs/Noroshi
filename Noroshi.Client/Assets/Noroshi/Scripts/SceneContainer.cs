using System;

namespace Noroshi
{
	public class SceneContainer
	{
		static Container _container = new Container();

		public static bool Contains<T>()
		{
			return _container.Contains<T>();
		}
		public static T Get<T>()
		{
			return _container.Get<T>();
		}
		public static void Register<T>(Func<T> factory)
		{
			_container.SetFactory<T>(factory);
		}
		public static void Clear()
		{
			_container = new Container();
		}
	}
}