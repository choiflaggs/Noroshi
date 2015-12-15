using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniLinq;
using Noroshi.BattleScene.UI;
using Noroshi.BattleScene.Camera;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class Factory : MonoBehaviour, IFactory, ISoundFactory
    {
        const string CHARACTER_TEXT_UI_PATH = "Battle/UI/CharacterTextUI";
        const string CHARACTER_ABOVE_UI_PATH = "Battle/UI/CharacterAboveUI";

        [SerializeField] UIController _UIController;
        [SerializeField] StoryUIView _storyUIView;
        [SerializeField] CameraController _cameraController;

        protected IObservable<IT[]> _loadSpineViewFromResourceMulti<T, IT>(string path, int num)
            where T : SpineView, IT
        {
            var go = Resources.Load<GameObject>(path);
            var spineViews = new T[num];
            for (var i = 0; i < num; i++)
            {
                spineViews[i] = (Instantiate(go) as GameObject).GetComponent<T>();
            }

            // ここで初期化が終了するまで待つ
            return spineViews.Select(spineView => spineView.GetOnCompleteInitializeObservable().Select(_ => spineView)).WhenAll()
                .Cast<T[], IT[]>();
        }

        protected IObservable<IT> _loadSpineViewFromResourceAsync<T, IT>(string path)
            where T : SpineView, IT
        {
            return _loadComponentFromResourceAsync<T>(path)
                .SelectMany(spineView => spineView.GetOnCompleteInitializeObservable().Select(_ => spineView)).Cast<T, IT>();
        }

        protected IObservable<IT[]> _loadComponentFromResourceMulti<T, IT>(string path, int num)
            where T : UnityEngine.Component, IT
        {
            // 既存個別ロードを使いまわさずに、個別実装にしてちょっとでもパフォーマンスを稼ぐ。
            var go = Resources.Load<GameObject>(path);
            var components = new IT[num];
            for (var i = 0; i < num; i++)
            {
                components[i] = (IT)(Instantiate(go) as GameObject).GetComponent<T>();
            }
            return Observable.Return<IT[]>(components);
        }
        protected IObservable<T> _loadComponentFromResource<T>(string path)
            where T : UnityEngine.Component
        {
            return _instantiateGameObjectFromResource(path).Select(go => go.GetComponent<T>());
        }
        IObservable<GameObject> _instantiateGameObjectFromResource(string filePath)
        {
            return _loadObjectFromResource<GameObject>(filePath).Select(o => Instantiate(o) as GameObject);
        }
        protected IObservable<T> _loadObjectFromResource<T>(string path)
            where T : UnityEngine.Object
        {
            return Observable.Return<T>(Resources.Load<T>(path) as T);
        }

        protected IObservable<T> _loadComponentFromResourceAsync<T>(string path)
            where T : UnityEngine.Component
        {
            return _instantiateGameObjectFromResourceAsync(path).Select(go => go.GetComponent<T>());
        }
        IObservable<GameObject> _instantiateGameObjectFromResourceAsync(string filePath)
        {
            return _loadObjectFromResourceAsync<GameObject>(filePath).Select(o => Instantiate(o) as GameObject);
        }
        IObservable<T> _loadObjectFromResourceAsync<T>(string filePath)
            where T : UnityEngine.Object
        {
            return Observable.FromCoroutine<T>((observer, cancellationToken) => _loadObjectFromResourceCoroutine<T>(filePath, observer, cancellationToken));
        }
        IEnumerator _loadObjectFromResourceCoroutine<T>(string filePath, IObserver<T> observer, CancellationToken cancellationToken)
            where T : UnityEngine.Object
        {
            var resourceRequest = Resources.LoadAsync(filePath, typeof(T));
            while (!resourceRequest.isDone && !cancellationToken.IsCancellationRequested)
            {
                yield return null;
            }
            if (cancellationToken.IsCancellationRequested) yield break;
            observer.OnNext(resourceRequest.asset as T);
            observer.OnCompleted();
        }

        public IObservable<IModalUIView> BuildModalUIView(string name)
        {
            var path = string.Format("Battle/UI/Modal/{0}", name);
            return _loadComponentFromResourceAsync<ModalUIView>(path).Cast<ModalUIView, IModalUIView>();
        }
        public IObservable<IResultUIView> BuildResultUIView(string name)
        {
            var path = string.Format("Battle/UI/Result/{0}", name);
            return _loadComponentFromResourceAsync<ResultUIView>(path).Cast<ResultUIView, IResultUIView>();
        }

        public IObservable<IStoryUIView> BuildStoryUIView()
        {
            return Observable.Return<IStoryUIView>(_storyUIView);
        }

        public IObservable<IFieldView> BuildFieldView(uint fieldId)
        {
            var path = string.Format("Field/{0}/Field", fieldId);
            return _loadComponentFromResourceAsync<FieldView>(path).Cast<FieldView, IFieldView>();
        }

        public IObservable<ICharacterView> BuildCharacterView(uint characterId)
        {
            var path = string.Format("Character/{0}/Character", characterId);
            // 初回ロードのみなので同期ロード。
            return _loadComponentFromResource<CharacterView>(path).Cast<CharacterView, ICharacterView>();
        }
        public IObservable<ICharacterView> BuildCharacterViewForUI(uint characterId)
        {
            var path = string.Format("Character/{0}/Character", characterId);
            // 初回ロードのみなので同期ロード。
            return _loadComponentFromResource<CharacterView>(path)
                .Do(characterView => _cameraController.AddChildToMainCamera(characterView.GetTransform()))
                .Cast<CharacterView, ICharacterView>();
        }

        public IObservable<Actions.IActionView> BuildActionView(uint characterId)
        {
            var path = string.Format("Character/{0}/Battle/Action", characterId);
            return _loadComponentFromResourceAsync<ActionView>(path).Cast<ActionView, Actions.IActionView>();
        }
        public IObservable<Actions.IActionRelationView> BuildActionRelationView(uint characterId)
        {
            var path = string.Format("Character/{0}/Battle/ActionRelation", characterId);
            return _loadComponentFromResourceAsync<ActionRelationView>(path).Cast<ActionRelationView, Actions.IActionRelationView>();
        }
        public IObservable<IBulletView> BuildBulletView(uint characterId)
        {
            var path = string.Format("Character/{0}/Battle/Bullet", characterId);
            return _loadSpineViewFromResourceAsync<BulletView, IBulletView>(path);
        }
        public IObservable<IBulletView[]> BuildBulletViewMulti(uint characterId, int num)
        {
            var path = string.Format("Character/{0}/Battle/Bullet", characterId);
            /// 事前ロードで呼ばれるので同期ロード。
            return _loadSpineViewFromResourceMulti<BulletView, IBulletView>(path, num);
        }

        public IObservable<UI.IUIController> BuildUIController()
        {
            return Observable.Return<IUIController>((IUIController)_UIController);
        }

        public IObservable<UI.IOwnCharacterPanelUIView> BuildOwnCharacterPanelUIView(uint characterId, int skinLevel)
        {
            // 初回ロードのみなので同期ロード。
            return _loadComponentFromResource<OwnCharacterPanelUIView>(string.Format("Battle/UI/OwnCharacterPanelUI", characterId))
            .SelectMany(view =>
            {
                return _loadObjectFromResource<Sprite>(string.Format("Character/{0}/Battle/skillbutton_" + skinLevel.ToString(), characterId)).Select(sprite =>
                {
                    view.SetSprite(sprite);
                    return view;
                });
            })
            .Cast<OwnCharacterPanelUIView, IOwnCharacterPanelUIView>();
        }
        public IObservable<Sprite> BuildCharacterThumbSprite(uint characterId, int skinLevel)
        {
            var spritePath = string.Format("Character/{0}/thumb_" + skinLevel, characterId);
            return Observable.Return<Sprite>(Resources.Load<Sprite>(spritePath));
        }

        public IObservable<ICharacterAboveUIView> BuildCharacterAboveUIView()
        {
            return _loadComponentFromResourceAsync<CharacterAboveUIView>(CHARACTER_ABOVE_UI_PATH).Cast<CharacterAboveUIView, ICharacterAboveUIView>();
        }
        public IObservable<ICharacterAboveUIView[]> BuildCharacterAboveUIViewMulti(int num)
        {
            /// 事前ロードで呼ばれるので同期ロード。
            return _loadComponentFromResourceMulti<CharacterAboveUIView, ICharacterAboveUIView>(CHARACTER_ABOVE_UI_PATH, num);
        }

        public IObservable<UI.ICharacterTextUIView> BuildCharacterTextUI()
        {
            return _loadComponentFromResourceAsync<CharacterTextUIView>(CHARACTER_TEXT_UI_PATH).Cast<CharacterTextUIView, ICharacterTextUIView>();
        }
        public IObservable<UI.ICharacterTextUIView[]> BuildCharacterTextUIMulti(int num)
        {
            /// 事前ロードで呼ばれるので同期ロード。
            return _loadComponentFromResourceMulti<CharacterTextUIView, ICharacterTextUIView>(CHARACTER_TEXT_UI_PATH, num);
        }

        public IObservable<ICharacterEffectView> BuildCharacterEffectView(uint prefabId)
        {
            var path = string.Format("CharacterEffect/{0}/CharacterEffect", prefabId);
            return _loadSpineViewFromResourceAsync<CharacterEffectView, ICharacterEffectView>(path);
        }
        public IObservable<ICharacterEffectView[]> BuildCharacterEffectViewMulti(uint prefabId, int num)
        {
            var path = string.Format("CharacterEffect/{0}/CharacterEffect", prefabId);
            /// 事前ロードで呼ばれるので同期ロード。
            return _loadSpineViewFromResourceMulti<CharacterEffectView, ICharacterEffectView>(path, num);
        }

        public IObservable<IDropItemUIView> BuildDropItemUIView(uint itemId)
        {
            var path = "Battle/UI/DropItemUI";
            // 事前ロードなので同期。
            return _loadComponentFromResource<DropItemUIView>(path).Cast<DropItemUIView, IDropItemUIView>();
        }
        public IObservable<IDropMoneyUIView> BuildDropMoneyUIView()
        {
            var path = "Battle/UI/DropMoneyUI";
            return _loadComponentFromResourceAsync<DropMoneyUIView>(path).Cast<DropMoneyUIView, IDropMoneyUIView>();
        }

        /* カメラ */

        public IObservable<ICameraController> BuildCameraController()
        {
            return Observable.Return<ICameraController>((ICameraController)_cameraController);
        }

        /* サウンド */

        const string BGM_PLAYER_PATH = "Sound/BgmPlayer";
        const string SE_PLAYER_PATH = "Sound/SEPlayer";
        const string VOICE_PLAYER_PATH = "Sound/VoicePlayer";
        const string SOUND_DIR = "Sound/";

        public IObservable<ISoundPlayer> LoadBgmPlayer()
        {
            // 事前ロードなので同期。
            return _loadComponentFromResource<SoundPlayer>(BGM_PLAYER_PATH).Cast<SoundPlayer, ISoundPlayer>();
        }
        public IObservable<ISoundPlayer> LoadSEPlayer()
        {
            // 事前ロードなので同期。
            return _loadComponentFromResource<SoundPlayer>(SE_PLAYER_PATH).Cast<SoundPlayer, ISoundPlayer>();
        }
        public IObservable<ISoundPlayer> LoadVoicePlayer()
        {
            // 事前ロードなので同期。
            return _loadComponentFromResource<SoundPlayer>(VOICE_PLAYER_PATH).Cast<SoundPlayer, ISoundPlayer>();
        }
        public IObservable<ISound[]> LoadSounds(IEnumerable<Core.WebApi.Response.Master.Sound> soundMasters)
        {
            var sounds = soundMasters.Select(s => new Sound(s.ID, s.Path, s.PlayType));
            return Observable.WhenAll(sounds.Select(sound => _loadSounds(sound.GetPathes()).Do(acs => sound.SetAudioClips(acs)).Select(_ => (ISound)sound)));
        }
        IObservable<AudioClip[]> _loadSounds(IEnumerable<string> paths)
        {
            // 事前ロードなので同期。
            return paths.Select(path => _loadObjectFromResource<AudioClip>(SOUND_DIR + path)).WhenAll();
        }
    }
}
