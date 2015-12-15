using System;
using UniRx;
using Noroshi.Repositories;
using Noroshi.Repositories.Server;
using Noroshi.WebApi;
using Noroshi.Master;
using Noroshi.Localization;
using Noroshi.TimeUtil;
using Noroshi.Random;

namespace Noroshi
{
	public class GlobalContainer
	{
        const string DEFAULT_API_HOST = "http://dev.jupiter-noroshi.net";
		protected static Container _instance;

		static GlobalContainer()
		{
			_instance = new Container();
			_initialize();
		}

		public static void SetFactory<T>(Func<T> factory)
		{
			_instance.SetFactory<T>(factory);
		}
		
		public static T Get<T>()
		{
			return _instance.Get<T>();
		}

        public static void Clear()
        {
            _instance.Clear();
        }
		
		protected static new void _initialize()
		{
			SetFactory<CharacterRepository>(() => new CharacterRepository());
			SetFactory<ITimeHandler>(() => new TimeHandler());
            SetFactory<Logger>(() => new Logger());
            SetFactory<IWebApiRequester>(() => new WebApiRequester());
            SetFactory<MasterManager>(() => new MasterManager());
            SetFactory<LocalizationManager>(() => new LocalizationManager());

            _tryToLoadLocalConfig();
		}

		public static RepositoryManager RepositoryManager { get { return Get<RepositoryManager>(); } }
        public static ITimeHandler TimeHandler { get { return Get<ITimeHandler>(); } }
		public static Logger Logger { get { return Get<Logger>(); } }
        public static IWebApiRequester WebApiRequester { get { return Get<IWebApiRequester>(); } }
        public static MasterManager MasterManager { get { return Get<MasterManager>(); } }
        public static LocalizationManager LocalizationManager { get { return Get<LocalizationManager>(); } }
        public static IRandomGenerator RandomGenerator { get { return Get<IRandomGenerator>(); } }
        public static Config Config { get { return Get<Config>(); } }

        public static IObservable<bool> Load()
        {
            return MasterManager.LoadAll(WebApiRequester)
                .SelectMany(_ => LocalizationManager.Load())
                .Select(_ => true);
        }

        static void _tryToLoadLocalConfig()
        {
            var filePath = "local";
            var textAsset = UnityEngine.Resources.Load(filePath) as UnityEngine.TextAsset;
            if (textAsset != null)
            {
                var localConfig = LitJson.JsonMapper.ToObject<Config>(textAsset.text);
                SetFactory<Config>(() => localConfig);
            }
            else
            {
                SetFactory<Config>(() => new Config(DEFAULT_API_HOST));
            }
        }
    }
    public class Config
    {
        public Config()
        {
        }
        public Config(string webApiHost)
        {
            WebApiHost = webApiHost;
        }
        public string WebApiHost { get; private set; }
    }
}