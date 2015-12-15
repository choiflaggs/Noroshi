using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class CharacterAboveUISet
    {
        Dictionary<Character, CharacterAboveUI> _characterAboveUIs = new Dictionary<Character, CharacterAboveUI>();

        public IObservable<CharacterAboveUISet> LoadAssets(IFactory factory)
        {
            return SceneContainer.GetCacheManager().PreloadCharacterAboveUIViewCache(factory)
            .Select(_ => this);
        }

        public IObservable<ICharacterAboveUIView> LoadCharacterAboveUIView(IUIController uiController, Character character)
        {
            return LoadCharacterAboveUIViews(uiController, new List<Character>{character}).Select(cs => cs.First());
        }

        public IObservable<ICharacterAboveUIView[]> LoadCharacterAboveUIViews(IUIController uiController, IEnumerable<Character> characters)
        {
            foreach (var character in characters)
            {
                var ui = new CharacterAboveUI();
                _characterAboveUIs.Add(character, ui);
            }
            return Observable.WhenAll(_characterAboveUIs.Select(kv =>
            {
                var character = kv.Key;
                var ui = kv.Value;
                return ui.LoadView(uiController).Do(v => _onLoadCharacterAboveUIViews(character, ui));
            }));
        }

        void _onLoadCharacterAboveUIViews(Character character, CharacterAboveUI ui)
        {
            ui.SetTarget(character);
        }

        public void RemoveCharacterAboveUIView(Character character)
        {
            _characterAboveUIs[character].Stock();
            _characterAboveUIs.Remove(character);
        }

        public void Clear()
        {
            foreach (var ui in _characterAboveUIs.Values)
            {
                ui.Stock();
            }
            _characterAboveUIs.Clear();
        }
    }
}
