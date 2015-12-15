using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.WebApi.Response.Story;

namespace Noroshi.UI {
    public class StoryController : MonoBehaviour {
        [SerializeField] StageScrollController normalWorld;
        [SerializeField] GameObject eliteWorld;
        [SerializeField] GameObject normalContent;
        [SerializeField] GameObject eliteContent;
        [SerializeField] BtnCommon btnBackToPartSelect;
        [SerializeField] BtnCommon btnNormal;
        [SerializeField] BtnCommon btnElite;
        [SerializeField] StorySelect storySelect;
        [SerializeField] StageInfo stageInfoModal;
        [SerializeField] AlertModal noStaminaAlert;

        public static int worldType;

        private bool isLoad = false;
        private uint[] defaultCharacterIdList;
        private uint[] requireCharacterIdList;
        private uint[] cpuCharacterIdList;
        private List<GameObject> normalEpisodeList = new List<GameObject>();
        private StoryEpisode storyEpisode;
        private uint stageID;
        private uint stamina;

        private void Start() {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.STORY);
            }

            btnBackToPartSelect.OnClickedBtn.Subscribe(_ => {
                normalWorld.SetPosition(0);
                storySelect.OpenStorySelect();
                stageInfoModal.gameObject.SetActive(false);
                Destroy(normalEpisodeList[0].gameObject);
                normalEpisodeList.RemoveAt(0);
//                Destroy(eliteChapterList[0].gameObject);
//                eliteChapterList.RemoveAt(0);
                UILoading.Instance.RemoveQuery(QueryKeys.SelectedChapter);
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnNormal.OnClickedBtn.Subscribe(SwitchWorld);

            btnElite.OnClickedBtn.Subscribe(SwitchWorld);

            storySelect.OnSelectEpisode.Subscribe(data => {
                CreateEpisodeMap(data.ChapterID, data.EpisodeID, data.StageDataList, data.ChapterIndex, data.EpisodeIndex);
            });

            defaultCharacterIdList = BattleCharacterSelect.Instance.GetDefaultCharacter(SaveKeys.DefaultStoryBattleCharacter);
            stageInfoModal.OnEditCharacter.Subscribe(id => {
                if(PlayerInfo.Instance.CurrentStamina < stamina) {
                    noStaminaAlert.OnOpen();
                    return;
                }
                stageID = (uint)id;
                BattleCharacterSelect.Instance.OpenPanel(false,
                    defaultCharacterIdList,
                    requireCharacterIdList,
                    cpuCharacterIdList
                );
            });

            stageInfoModal.OnOpenStageInfo.Subscribe(index => {
                var posY = storyEpisode.GetTargetPosition(index);
                normalWorld.TransitionPosition(-posY + 50);
            });

            normalWorld.OnStartDrag.Subscribe(_ => {
                stageInfoModal.CloseStageInfo();
            });

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultStoryBattleCharacter, playerCharacterIds);
                BattleScene.Bridge.Transition.TransitToCpuBattle(BattleCategory.Stage, stageID, playerCharacterIds);
            }).AddTo(this);

            BattleCharacterSelect.Instance.OnClosePanel.Subscribe(list => {
                stageInfoModal.gameObject.SetActive(true);
            }).AddTo(this);

            SetStageData();

            BattleCharacterSelect.Instance.ReloadCharacterList();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad || !BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void SetStageData() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            var gcrm = GlobalContainer.RepositoryManager;
            uint selectedChapter = 0;
            uint selectedEpisode = 0;
            uint selectedStage = 0;
            gcrm.PlayerStoryChapterRepository.GetAllAndMaster().Do(chapterList => {
                gcrm.PlayerStoryEpisodeRepository.GetLastStage().Do(lastPlayStage => {
                    Debug.Log("Chapter: " + lastPlayStage.ChapterID + ", Episode: " + lastPlayStage.EpisodeID + ", Stage: " + lastPlayStage.StageID);
                    for(int i = 0, l = chapterList.Length; i < l; i++) {
                        storySelect.SetChapterData(chapterList[i]);
                    }
                    if(UILoading.Instance.GetQuery(QueryKeys.SelectedChapter) > -1) {
                        selectedChapter = (uint)UILoading.Instance.GetQuery(QueryKeys.SelectedChapter);
                        selectedEpisode = (uint)UILoading.Instance.GetQuery(QueryKeys.SelectedEpisode);
                        selectedStage = (uint)UILoading.Instance.GetQuery(QueryKeys.SelectedStage);
                    } else {
                        selectedChapter = lastPlayStage.ChapterID;
                        selectedEpisode = lastPlayStage.EpisodeID;
                        selectedStage = lastPlayStage.StageID;
                    }
                    storySelect.SetDefaultChapterPanel(selectedChapter, selectedEpisode);
                    if(selectedEpisode != 0) {
                        for(int i = 0, iz = chapterList.Length; i < iz; i++) {
                            if(chapterList[i].Chapter.ID == selectedChapter) {
                                for(int j = 0, jz = chapterList[i].Episodes.Length; j < jz; j++) {
                                    if(chapterList[i].Episodes[j].PlayerEpisode.EpisodeID == selectedEpisode) {
                                        var stageDataList = chapterList[i].Episodes[j].PlayerEpisode.PlayerStageList;
                                        storySelect.CloseStorySelect();
                                        storySelect.SetChapterAndEpisodeIndex(i, j);
                                        CreateEpisodeMap(selectedChapter, selectedEpisode, stageDataList, i, j, selectedStage);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    isLoad = true;
                }).Subscribe();
            }).Subscribe();
        }

        private void CreateEpisodeMap(uint chapterID, uint episodeID, PlayerStoryStage[] stageDataList, int chapterIndex, int episodeIndex, uint stageID = 0, int type = 0) {
            GameObject episodeMap;
            if(type == 0) {
                episodeMap = Instantiate(Resources.Load(
                    string.Format("Story/Chapter{0}/Normal/Episode{1}", chapterID, episodeID)
                )) as GameObject;
                episodeMap.transform.SetParent(normalContent.transform);
                normalEpisodeList.Add(episodeMap);
            } else {
                episodeMap = Instantiate(Resources.Load(
                    string.Format("Story/Chapter{0}/Elite/Episode{1}", chapterID, episodeID)
                )) as GameObject;
                episodeMap.transform.SetParent(eliteContent.transform);
            }
            episodeMap.transform.localScale = Vector3.one;
            episodeMap.transform.localPosition = Vector3.zero;

            storyEpisode = episodeMap.GetComponent<StoryEpisode>();
            storyEpisode.OnStageClicked.Subscribe(stageData => {
                stamina = (uint)stageData.Stamina;
                requireCharacterIdList = stageData.RequireIDList;
                cpuCharacterIdList = stageData.CPUIDList;
                stageInfoModal.SetStageInfo(stageData);
            }).AddTo(storyEpisode);
            storyEpisode.SetEpisodeMap(episodeID, stageID, chapterIndex, episodeIndex, stageDataList);
            stageInfoModal.gameObject.SetActive(true);
        }

        private void SwitchWorld(int index) {
            worldType = index;
            if(index == 0) {
                normalWorld.gameObject.SetActive(true);
                TweenA.Add(normalWorld.gameObject, 0.75f, 1);
                TweenA.Add(eliteWorld.gameObject, 0.75f, 0).Then(() => {
                    eliteWorld.SetActive(false);
                });
            } else {
                TweenA.Add(normalWorld.gameObject, 0.75f, 0).Then(() => {
                    normalWorld.gameObject.SetActive(false);
                });
                eliteWorld.SetActive(true);
                TweenA.Add(eliteWorld, 0.75f, 1);
            }
        }
    }
}


