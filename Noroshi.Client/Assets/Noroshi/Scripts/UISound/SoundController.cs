using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Noroshi.UI {
    public class SoundController : MonoBehaviour {
        public class BGMKeys {
            public const string HOME = "Home";
            public const string STORY = "Story";
            public const string ARENA = "Arena";
            public const string CHARACTER_LIST = "CharacterList";
        }

        public class SEKeys {
            public const int DECIDE = 0;
            public const int CANCEL = 1;
            public const int SELECT = 2;
            public const int GET = 3;
            public const int ERROR = 4;
            public const int MENU = 5;
            public const int PAY = 6;
            public const int LEVEL_UP = 7;
            public const int EQUIP = 8;
            public const int EVOLUTION = 9;
            public const int UPGRADE = 10;
            public const int STATUS_UP = 11;
        }

        static SoundController instance;

        public static SoundController Instance {
            get {
                if(instance == null) {
                    instance = (SoundController)FindObjectOfType(typeof(SoundController));
                }
                return instance;
            }
        }

        [SerializeField] AudioClip[] seList;
        [SerializeField] int synchSENum = 10;

        AudioSource bgmSource;
        AudioSource introSource;
        AudioSource[] seSourceList;
        string playingBGM;
        bool isSoundOn = true;

        void Awake() {
            GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundController");
            if(obj.Length > 1) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }
            bgmSource = new AudioSource();
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            introSource = new AudioSource();
            introSource = gameObject.AddComponent<AudioSource>();
            seSourceList = new AudioSource[synchSENum];
            for(int i = 0; i < synchSENum; i++) {
                seSourceList[i] = gameObject.AddComponent<AudioSource>();
            }

    //        if(data.GetValue(Keys.isSoundOn) == 1) {
    //            isSoundOn = true;
    //        } else {
    //            isSoundOn = false;
    //            StopBGM();
    //        }
        }

        void OnLevelWasLoaded(int level) {
            if(Application.loadedLevelName == Constant.SCENE_BATTLE) {
                Destroy(gameObject);
            }
        }

        IEnumerator SetAndStartIntro(string bgmName) {
            AudioClip introClip = Resources.Load<AudioClip>("Sound/Intro/" + bgmName);
            AudioClip bgmClip = Resources.Load<AudioClip>("Sound/BGM/" + bgmName);

            while(introClip.loadState != AudioDataLoadState.Loaded ||
                bgmClip.loadState != AudioDataLoadState.Loaded) {
                yield return new WaitForEndOfFrame();
            }
            introSource.clip = introClip;
            bgmSource.clip = bgmClip;
            if(bgmName == Constant.SCENE_CHARACTER_LIST) {
                introSource.volume = 0.5f;
                bgmSource.volume = 0.5f;
            } else {
                introSource.volume = 1;
                bgmSource.volume = 1;
            }
            TweenNull.Add(gameObject, 0.2f).Then(() => {
                introSource.Play();
                bgmSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
            });
        }

        IEnumerator SetAndStartBGM(string bgmName) {
            AudioClip clip = Resources.Load<AudioClip>("Sound/BGM/" + bgmName);

            while(clip.loadState != AudioDataLoadState.Loaded) {
                yield return new WaitForEndOfFrame();
            }
            bgmSource.clip = clip;
            if(bgmName == Constant.SCENE_CHARACTER_LIST) {
                bgmSource.volume = 0.5f;
            } else {
                bgmSource.volume = 1;
            }
            bgmSource.Play();
        }

        public void EnableSound() {
            isSoundOn = true;
        }

        public void DisableSound() {
            isSoundOn = false;
            StopBGM();
        }

        public void PlayBGM(string bgmName, bool isIntro = false) {
            if(!isSoundOn) {return;}
            if(playingBGM == bgmName) {
                return;
            } else {
                playingBGM = bgmName;
                introSource.Stop();
                bgmSource.Stop();
                if(isIntro) {
                    StartCoroutine(SetAndStartIntro(bgmName));
                } else {
                    StartCoroutine(SetAndStartBGM(bgmName));
                }
            }
        }

        public void PauseBGM() {
            if(bgmSource.isPlaying) {bgmSource.Pause();}
        }

        public void StopBGM() {
            if(bgmSource.isPlaying) {
                bgmSource.Pause();
            } else {
                bgmSource.time = 0;
            }
        }

        public void VolumeDown(int index) {
            bgmSource.volume = 0.5f;
        }

        public void VolumeUp(int index) {
            bgmSource.volume = 1.0f;
        }

        public void PlaySE(int index) {
            if(!isSoundOn || index < 0 || index >= seList.Length) {return;}
            foreach(AudioSource source in seSourceList) {
                if(!source.isPlaying){
                    source.clip = seList[index];
                    source.Play();
                    return;
                }
            }
            seSourceList[0].Stop();
            seSourceList[0].clip = seList[index];
            seSourceList[0].Play();
        }

        public void StopSE(){
            foreach(AudioSource source in seSourceList) {
                source.Stop();
                source.clip = null;
            }  
        }

        public void PlayVoice(uint characterID, string animType) {
            if(animType == Constant.ANIM_RUN || animType == Constant.ANIM_WALK) {
                animType = "start";
            }
            AudioClip[] voiceClipList = Resources.LoadAll<AudioClip>(
                "Sound/Character/" + characterID + "/" + animType
            );
            if(voiceClipList.Length < 1) {return;}
            var voiceClip = voiceClipList[UnityEngine.Random.Range(0, voiceClipList.Length)];
            foreach(AudioSource source in seSourceList) {
                if(!source.isPlaying){
                    source.clip = voiceClip;
                    source.Play();
                    return;
                }
            }
        }
    }
}
