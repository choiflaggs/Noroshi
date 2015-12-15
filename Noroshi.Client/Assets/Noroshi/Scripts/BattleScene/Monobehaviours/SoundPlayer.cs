using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Noroshi.BattleScene.Sound;
using Noroshi.Core.Game.Sound;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour, ISoundPlayer
    {
        [SerializeField] byte _audioSourceNum = 1;
        AudioSource[] _audioSources;
        Dictionary<ISound, Subject<bool>> _onCompleteSubjects = new Dictionary<ISound, Subject<bool>>();

        new void Awake()
        {
            base.Awake();
            var audioSource = GetComponent<AudioSource>();

            var audioSources = new List<AudioSource>{audioSource};
            for (var i = 1; i < _audioSourceNum; i++)
            {
                audioSources.Add(_gameObject.AddComponent<AudioSource>());
            }
            _audioSources = audioSources.ToArray();
        }

        public IObservable<bool> Play(ISound sound)
        {
            switch (sound.PlayType)
            {
                case PlayType.Loop:
                    return _play(sound);
                case PlayType.LoopWithIntro:
                    return _playWithIntro(sound);
                case PlayType.OneShot:
                    return _playOneShot(sound);
                case PlayType.Jingle:
                    return _play(sound);
                default:
                    throw new InvalidOperationException();
            }
        }

        IObservable<bool> _play(ISound sound)
        {
            if (_onCompleteSubjects.ContainsKey(sound))
            {
                Stop(sound);
            }
            _onCompleteSubjects.Add(sound, new Subject<bool>());
            _audioSources[0].clip = sound.GetAudioClip();
            _audioSources[0].Play();
            return _onCompleteSubjects[sound].AsObservable();
        }

        IObservable<bool> _playWithIntro(ISound sound)
        {
            if (_onCompleteSubjects.ContainsKey(sound))
            {
                Stop(sound);
            }
            _onCompleteSubjects.Add(sound, new Subject<bool>());
            _audioSources[0].clip = sound.GetIntro();
            _audioSources[0].Play();
            _audioSources[0].loop = false;

            _audioSources[1].clip = sound.GetAudioClip();
            _audioSources[1].PlayDelayed(_audioSources[0].clip.length);
            _audioSources[1].loop = true;

            return _onCompleteSubjects[sound].AsObservable();
        }
        IObservable<bool> _playOneShot(ISound sound)
        {
            _audioSources[0].PlayOneShot(sound.GetAudioClip());
            return Observable.Timer(TimeSpan.FromSeconds(sound.GetLength().Value)).Select(_ => true);
        }

        public void Stop(ISound sound)
        {
            _stop(sound, false);
        }
        void _stop(ISound sound, bool success)
        {
            _onCompleteSubjects[sound].OnNext(success);
            _onCompleteSubjects[sound].OnCompleted();
            _onCompleteSubjects.Remove(sound);
        }
    }
}
