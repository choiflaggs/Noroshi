using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response.Story;

namespace Noroshi.UI {
    public class ChapterPanel : MonoBehaviour {
        public class SelectedEpisodeData {
            public int Index;
            public PlayerStoryStageAndPlayerStoryEpisodeResponse StageAndEpisodeData;
        }

        [SerializeField] BtnEpisode btnEpisodePref;
        [SerializeField] GameObject episodeBtnWrapper;
        [SerializeField] Text txtTitle;
        [SerializeField] Text txtDescription;
        [SerializeField] BtnCommon btnNext;

        public Subject<SelectedEpisodeData> OnSelectEpisode = new Subject<SelectedEpisodeData>();


        private List<BtnEpisode> btnEpisodeList = new List<BtnEpisode>();
        private PlayerStoryStageAndPlayerStoryEpisodeResponse[] episodeDataList;

        private void Start() {
            btnNext.OnClickedBtn.Subscribe(index => {
                if(episodeDataList[index].PlayerEpisode != null) {
                    SelectEpisode(index);
                } else {
                    var gcrm = GlobalContainer.RepositoryManager;
                    var episodeID = episodeDataList[index].Episode.ID;
                    btnNext.SetEnable(false);
                    gcrm.PlayerStoryEpisodeRepository.ChangeLastEpisode(episodeID).Do(episodeData => {
                        btnNext.SetEnable(true);
                        SelectEpisode(index);
                    }).Subscribe();
                }
            });
            btnNext.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        private void SelectEpisode(int index) {
            var data = new SelectedEpisodeData();
            data.Index = index;
            data.StageAndEpisodeData = episodeDataList[index];
            OnSelectEpisode.OnNext(data);
        }

        private void SetEpisodeBtn(int index) {
            if(episodeDataList[index].PlayerEpisode == null) {
                btnEpisodeList[index].SetBtnState(true, true);
            } else if(episodeDataList[index].PlayerEpisode.IsClearEpisode) {
                btnEpisodeList[index].SetBtnState(true, false);
                if(index < episodeDataList.Length - 1) {
                    SetEpisodeBtn(index + 1);
                } else {
                    btnEpisodeList[0].SetBtnState(true, true);
                }
            } else {
                btnEpisodeList[index].SetBtnState(true, true);
            }
        }

        private void SetSelectedBtn(int index) {
            for(int i = 0, l = btnEpisodeList.Count; i < l; i++) {
                if(i == index) {
                    btnEpisodeList[i].SetSelected(true);
                } else {
                    btnEpisodeList[i].SetSelected(false);
                }
            }
        }

        public void SetSelectedEpisodeBtn(int index) {
            btnEpisodeList[index].OnClickedBtn.OnNext(index);
        }

        public void SetChapterPanelInfo(PlayerStoryStageAndPlayerStoryEpisodeResponse[] episodes, string chapterName) {
            txtTitle.text = GlobalContainer.LocalizationManager.GetText(chapterName + ".Name");
            episodeDataList = episodes;
            for(int i = 0, l = episodes.Length; i < l; i++) {
                var btn = Instantiate(btnEpisodePref);
                btn.SetBtnEpisode(episodes[i].Episode, i);
                btn.SetBtnState(false, false);
                btn.transform.SetParent(episodeBtnWrapper.transform);
                btn.transform.localScale = Vector3.one;
                btn.OnClickedBtn.Subscribe(index => {
                    btnNext.id = index;
                    txtDescription.text = GlobalContainer.LocalizationManager.GetText(
                        episodeDataList[index].Episode.TextKey + ".Description"
                    );
                    SetSelectedBtn(index);
                });
                btn.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
                btnEpisodeList.Add(btn);
            }
            SetEpisodeBtn(0);
        }
    }
}
