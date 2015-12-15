using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.Game.Player;
using Noroshi.Core.Game.GameContent;

namespace Noroshi.UI {
    public class MyPageController : MonoBehaviour {
        [SerializeField] GameObject splashImg;
        [SerializeField] GameObject playerInfo;
        [SerializeField] GameObject menu;
        [SerializeField] BtnCommon btnOpenPlayerStatus;
        [SerializeField] PlayerStatusUnit playerStatusUnit;
        [SerializeField] BtnLoadLevel[] btnLoadList;

        private bool isLoad = false;

        private void Awake() {
            splashImg.SetActive(true);
            TweenNull.Add(gameObject, 0.001f).Then(() => {
                if(UILoading.Instance.HistoryList.Count > 0) {
                    splashImg.SetActive(false);
                    Init();
                    UILoading.Instance.ShowLoading();
                } else {
                    Noroshi.WebApi.WebApiRequester.Login()
                    .SelectMany(_ => GlobalContainer.Load())
                    .Do(id => {
                        splashImg.SetActive(false);
                        Init();
                        UILoading.Instance.ShowLoading();
                    }).Subscribe();
                }
            });
        }

        private void Init() {
            playerInfo.SetActive(true);
            menu.SetActive(true);

            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.HOME);
            }

            if(!BattleCharacterSelect.Instance.isLoad) {
                BattleCharacterSelect.Instance.OpenPanel(true);
                BattleCharacterSelect.Instance.LoadCharacterList();
            }
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            GlobalContainer.RepositoryManager.PlayerStatusRepository.Get().Do(data => {
                var openGameContents = Core.Game.GameContent.GameContent.BuildOpenGameContentsByPlayerLevel(data.Level);
                foreach(var d in openGameContents) {
                    CheckUnLock((GameContentID)d.ID);
                }
                btnLoadList[0].SetEnable(true);
                btnLoadList[6].SetEnable(true);
                playerStatusUnit.SetPlayerStatus(data);
                isLoad = true;
            }).Subscribe();

            PlayerInfo.Instance.OnUpPlayerLevel.Subscribe(contents => {
                foreach(var content in contents) {
                    CheckUnLock((GameContentID)content.ID);
                }
            }).AddTo(this);

            PlayerInfo.Instance.OnQuestComplete.Subscribe(_ => {
                GlobalContainer.RepositoryManager.PlayerStatusRepository.Get().Do(data => {
                    playerStatusUnit.SetPlayerStatus(data);
                }).Subscribe();
            }).AddTo(this);

            StartCoroutine("OnLoading");

            // アニメーションデータをメモリに載せてしまう。
//            foreach (var skeletonDataAsset in Resources.LoadAll<SkeletonDataAsset>("Character"))
//            {
//                skeletonDataAsset.GetSkeletonData(true);
//            }
//            foreach (var skeletonDataAsset in Resources.LoadAll<SkeletonDataAsset>("CharacterEffect"))
//            {
//                skeletonDataAsset.GetSkeletonData(true);
//            }
        }

        private IEnumerator OnLoading() {
            while(!isLoad || !BattleCharacterSelect.Instance.isLoad || !PlayerInfo.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            BattleCharacterSelect.Instance.ClosePanel();
            if(UILoading.Instance != null) {
                TweenNull.Add(gameObject, 0.3f).Then(() => {
                    UILoading.Instance.HideLoading();
                });
            }
            if(PlayerInfo.Instance.GetTutorialStep() >= TutorialStep.ClearStoryStage7) {
                btnLoadList[1].SetEnable(true);
            }
        }

        private void CheckUnLock(GameContentID categoryName) {
            switch(categoryName) {
                case GameContentID.Arena: btnLoadList[2].SetEnable(true); break;
                case GameContentID.Training: btnLoadList[3].SetEnable(true); break;
                case GameContentID.Trial: btnLoadList[4].SetEnable(true); break;
                case GameContentID.Expedition: btnLoadList[5].SetEnable(true); break;
                //              case ContentsCategory.Shop: btnLoadList[6].SetEnable(true); break;
                case GameContentID.BeginnerGuild: btnLoadList[7].SetEnable(true); break;
                case GameContentID.NormalGuild: btnLoadList[7].SetEnable(true); btnLoadList[8].SetEnable(true); break;
                default: break;
            }
        }
    }
}
