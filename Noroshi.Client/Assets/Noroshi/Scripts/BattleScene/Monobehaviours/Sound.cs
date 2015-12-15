using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using UnityEngine;
using Noroshi.Core.Game.Sound;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class Sound : ISound
    {
        string _path;
        AudioClip[] _audioClips;
        public Sound(uint id, string path, PlayType playType)
        {
            ID = id;
            _path = path;
            PlayType = playType;
        }
        
        public uint ID { get; private set; }
        public PlayType PlayType { get;  private set; }

        public IEnumerable<string> GetPathes()
        {
            return PlayType == PlayType.LoopWithIntro ? new[] { _path + "_intro", _path } : new[] { _path };
        }
        public void SetAudioClips(AudioClip[] audioClips)
        {
            _audioClips = audioClips;
        }
        
        public AudioClip GetAudioClip()
        {
            return _audioClips.Last();
        }
        public AudioClip GetIntro()
        {
            if (PlayType != PlayType.LoopWithIntro) throw new InvalidOperationException();
            return _audioClips.First();
        }
        public float? GetLength()
        {
            if (PlayType == PlayType.Loop || PlayType == PlayType.LoopWithIntro) return null;
            return (float?)_audioClips[0].length;
        }
    }
}
