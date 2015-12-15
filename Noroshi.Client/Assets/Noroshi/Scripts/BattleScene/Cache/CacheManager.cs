using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.Cache
{
    public class CacheManager : IManager
    {
        const int BULLET_VIEW_PRELOAD_NUM = 5;
        const int CHARACTER_EFFECT_VIEW_PRELOAD_NUM = 10;
        const int CHARACTER_ABOVE_UI_VIEW_PRELOAD_NUM = 20;
        const int CHARACTER_TEXT_UI_VIEW_PRELOAD_NUM = 30;

        Dictionary<uint, BulletCache> _bulletCaches = new Dictionary<uint, BulletCache>();
        Dictionary<uint, CharacterEffectCache> _characterEffectCaches = new Dictionary<uint, CharacterEffectCache>();
        CharacterAboveUIViewCache _characterAboveUIViewCache;
        CharacterTextUIViewCache _characterTextUIViewCache;

        public void Initialize()
        {
        }
        public IObservable<IManager> LoadDatas()
        {
            return Observable.Return<IManager>(this);
        }
        public IObservable<IManager> LoadAssets(IFactory factory)
        {
            return PreloadCharacterTextUIViewCache(factory)
            .Select(_ => (IManager)this);
        }
        public void Prepare()
        {
        }

        public BulletCache GetBulletCache(uint actionId)
        {
            return _bulletCaches[actionId];
        }
        public IObservable<CacheManager> PreloadBulletView(IActionFactory factory, IEnumerable<uint> characterIds)
        {
            foreach (var characterId in characterIds.Where(id => !_bulletCaches.ContainsKey(id)))
            {
                _bulletCaches.Add(characterId, new BulletCache(factory, characterId));
            }
            return Observable.WhenAll(_bulletCaches.Values.Select(c => c.Preload(BULLET_VIEW_PRELOAD_NUM))).Select(_ => this);
        }

        public CharacterEffectCache GetCharacterEffectCache(uint prefabId)
        {
            return _characterEffectCaches[prefabId];
        }
        public CharacterEffectCache ForceGetCharacterEffectCache(ICharacterFactory factory, uint prefabId)
        {
            if (!_characterEffectCaches.ContainsKey(prefabId))
            {
                _characterEffectCaches.Add(prefabId, new CharacterEffectCache(factory, prefabId));
            }
            return _characterEffectCaches[prefabId];
        }
        public IObservable<CacheManager> PreloadCharacterEffectView(ICharacterFactory factory, IEnumerable<uint> prefabIds)
        {
            foreach (var prefabId in prefabIds)
            {
                _characterEffectCaches.Add(prefabId, new CharacterEffectCache(factory, prefabId));
            }
            return Observable.WhenAll(_characterEffectCaches.Values.Select(c => c.Preload(CHARACTER_EFFECT_VIEW_PRELOAD_NUM))).Select(_ => this);
        }

        public CharacterAboveUIViewCache GetCharacterAboveUIViewCache()
        {
            return _characterAboveUIViewCache;
        }
        public IObservable<CacheManager> PreloadCharacterAboveUIViewCache(IFactory factory)
        {
            _characterAboveUIViewCache = new CharacterAboveUIViewCache(factory);
            return _characterAboveUIViewCache.Preload(CHARACTER_ABOVE_UI_VIEW_PRELOAD_NUM).Select(_ => this);
        }

        public CharacterTextUIViewCache GetCharacterTextUIViewCache()
        {
            return _characterTextUIViewCache;
        }
        public IObservable<CacheManager> PreloadCharacterTextUIViewCache(IFactory factory)
        {
            _characterTextUIViewCache = new CharacterTextUIViewCache(factory);
            return _characterTextUIViewCache.Preload(CHARACTER_TEXT_UI_VIEW_PRELOAD_NUM).Select(_ => this);
        }

        public void Dispose()
        {
        }
    }
}