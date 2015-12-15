using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Sound;
using Noroshi.Master;

namespace Noroshi.BattleScene.Sound
{
    public class SoundManager : IManager
    {
        SoundMaster _soundMaster;
        IEnumerable<uint> _soundIds;
        ISoundPlayer _bgmPlayer;
        ISoundPlayer _sePlayer;
        ISoundPlayer _voicePlayer;
        Dictionary<uint, ISound> _sounds = new Dictionary<uint, ISound>();
        Dictionary<uint, bool> _playingSounds = new Dictionary<uint, bool>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public void Initialize()
        {
            SceneContainer.GetSceneManager().GetOnCommandSoundObservable()
            .Subscribe(e => _onCommandSound(e)).AddTo(_disposables);
        }

        public IObservable<IManager> LoadDatas()
        {
            _soundMaster = GlobalContainer.MasterManager.SoundMaster;
            // TODO : マスターから。
            _soundIds = new uint[]{1, 2, 1001, 1002, 2001};
            return Observable.Return<IManager>(this);
        }

        public IObservable<IManager> LoadAssets(IFactory factory)
        {
            return _loadAssets((ISoundFactory)factory);
        }
        public IObservable<IManager> _loadAssets(ISoundFactory factory)
        {
            return Observable.WhenAll(
                factory.LoadBgmPlayer().Do(sp => _bgmPlayer = sp),
                factory.LoadSEPlayer().Do(sp => _sePlayer = sp),
                factory.LoadVoicePlayer().Do(sp => _voicePlayer = sp)
            )
            .SelectMany(_ => factory.LoadSounds(_soundIds.Select(soundId => _soundMaster.Get(soundId))))
            .Do(ss => _sounds = ss.ToDictionary(s => s.ID))
            .Select(_ => (IManager)this);
        }
        public void Prepare()
        {
        }

        IObservable<bool> _onCommandSound(SoundEvent soundEvent)
        {
            IObservable<bool> observable;
            // 命令内容によって処理を分岐
            switch (soundEvent.Command)
            {
                case SoundCommand.Play:
                    observable = _play(soundEvent.SoundID);
                    break;
                case SoundCommand.Stop:
                    observable = _stop(soundEvent.SoundID);
                    break;
                default:
                    observable = Observable.Empty<bool>();
                    break;
            }
            return observable;
        }

        IObservable<bool> _play(uint soundId)
        {
            var sound = _sounds[soundId];
            var player = sound.PlayType == PlayType.OneShot ? _sePlayer : _bgmPlayer;
            return player.Play(sound);
        }
        
        IObservable<bool> _stop(uint soundId)
        {
            var sound = _sounds[soundId];
            _bgmPlayer.Stop(sound);
            return Observable.Return<bool>(false);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}