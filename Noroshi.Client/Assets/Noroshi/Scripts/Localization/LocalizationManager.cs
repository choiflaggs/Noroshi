using System.Collections.Generic;
using UniLinq;
using UniRx;
using LitJson;
using UnityEngine;

namespace Noroshi.Localization
{
    public class LocalizationManager
    {
        Dictionary<string, string> _textMap = new Dictionary<string, string>();

        public LocalizationManager()
        {
        }

        public string GetText(string id)
        {
            return _textMap.ContainsKey(id) ? _textMap[id] : "";
        }
        public IObservable<LocalizationManager> Load()
        {
            _textMap.Clear();
            var dynamicTexts = GlobalContainer.MasterManager.TextMaster.GetAllDynamicTexts();
            foreach (var dynamicText in dynamicTexts)
            {
                _textMap.Add("Dynamic." + dynamicText.ID, dynamicText.Text);
            }
            return _load("Game")
            .SelectMany(_ => _load("Character"))
            .SelectMany(_ => _load("UI"))
            .SelectMany(_ => _load("Master/Action"))
            .SelectMany(_ => _load("Master/Character"))
            .SelectMany(_ => _load("Master/CpuBattleStoryMessage"))
            .SelectMany(_ => _load("Master/DailyQuest"))
            .SelectMany(_ => _load("Master/Item"))
            .SelectMany(_ => _load("Master/LoginBonus"))
            .SelectMany(_ => _load("Master/Quest"))
            .SelectMany(_ => _load("Master/Shop"))
            .SelectMany(_ => _load("Master/StoryChapter"))
            .SelectMany(_ => _load("Master/StoryEpisode"))
            .SelectMany(_ => _load("Master/StoryStage"))
            .SelectMany(_ => _load("Master/Training"))
            .SelectMany(_ => _load("Master/Trial"))
            .Select(_ => this);
        }
        protected IObservable<Dictionary<string, string>> _load(string file)
        {
            var filePath = "Localization/JP/" + file;
            var prefix = string.Join(".", file.Split('/'));
            return _loatText(filePath).Select(t => JsonMapper.ToObject<Dictionary<string, string>>(t))
                .Select(dict => dict.ToDictionary(kv => prefix + "." + kv.Key, kv => kv.Value))
                .Do(map =>
                {
                    _textMap = _textMap.Concat(map).ToDictionary(kv => kv.Key, kv => kv.Value);
                });
        }
        protected IObservable<string> _loatText(string filePath)
        {
            var stageTextAsset = UnityEngine.Resources.Load(filePath) as TextAsset;
            return Observable.Return<string>(stageTextAsset.text);
        }
    }
}
