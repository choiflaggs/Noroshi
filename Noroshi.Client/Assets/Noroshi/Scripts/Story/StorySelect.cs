using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Noroshi.Core.WebApi.Response.Story;
using UniRx;

namespace Noroshi.UI {
    public class StorySelect : MonoBehaviour {
        public class EpisodeData {
            public uint ChapterID;
            public uint EpisodeID;
            public int ChapterIndex;
            public int EpisodeIndex;
            public PlayerStoryStage[] StageDataList;
        }

        [SerializeField] CarouselController carouselController;
        [SerializeField] GameObject chapterWrapper;
        [SerializeField] ChapterPanel chapterPanelPref;

        public Subject<EpisodeData> OnSelectEpisode = new Subject<EpisodeData>();

        private List<ChapterPanel> chapterPanelList = new List<ChapterPanel>();
        private List<StoryChapter> chapterDataList = new List<StoryChapter>();
        private int selectedChapterIndex = 0;
        private int selectedEpisodeIndex = 0;

        private void Start() {
            carouselController.OnMoveEnd.Subscribe(index => {
                selectedChapterIndex = index;
                for(int i = 0, l = chapterPanelList.Count; i < l; i++) {
                    if(selectedChapterIndex == 0) {
                        if(i == selectedChapterIndex || i == selectedChapterIndex + 1) {
                            chapterPanelList[i].gameObject.SetActive(true);
                        } else {
                            chapterPanelList[i].gameObject.SetActive(false);
                        }
                    } else if(selectedChapterIndex == l - 1) {
                        if(i == selectedChapterIndex || i == selectedChapterIndex - 1) {
                            chapterPanelList[i].gameObject.SetActive(true);
                        } else {
                            chapterPanelList[i].gameObject.SetActive(false);
                        }
                    } else {
                        if(i == selectedChapterIndex || i == selectedChapterIndex + 1 || i == selectedChapterIndex - 1) {
                            chapterPanelList[i].gameObject.SetActive(true);
                        } else {
                            chapterPanelList[i].gameObject.SetActive(false);
                        }
                    }
                }
            });
        }

        public void SetChapterData(StoryChapterAndStoryEpisodeResponse chapterAndEpisode) {
            var chapterPanel = Instantiate(chapterPanelPref);

            chapterDataList.Add(chapterAndEpisode.Chapter);
            chapterPanel.transform.SetParent(chapterWrapper.transform);
            chapterPanel.transform.localScale = Vector3.one;
            chapterPanel.SetChapterPanelInfo(chapterAndEpisode.Episodes, chapterAndEpisode.Chapter.TextKey);
            chapterPanel.OnSelectEpisode.Subscribe(data => {
                EpisodeData episodeData = new EpisodeData();
                episodeData.ChapterID = chapterDataList[selectedChapterIndex].ID;
                episodeData.EpisodeID = data.StageAndEpisodeData.Episode.ID;
                episodeData.ChapterIndex = selectedChapterIndex;
                episodeData.EpisodeIndex = data.Index;
                if(data.StageAndEpisodeData.PlayerEpisode == null) {
                    var gcrm = GlobalContainer.RepositoryManager;
                    var episodeID = data.StageAndEpisodeData.Episode.ID;
                    gcrm.PlayerStoryStageRepository.GetByEpisodeID(episodeID).Do(stages => {
                        episodeData.StageDataList = stages;
                        OnSelectEpisode.OnNext(episodeData);
                    }).Subscribe();
                } else {
                    episodeData.StageDataList = data.StageAndEpisodeData.PlayerEpisode.PlayerStageList;
                    OnSelectEpisode.OnNext(episodeData);
                }
                CloseStorySelect();
            });
            chapterPanelList.Add(chapterPanel);
        }

        public void SetDefaultChapterPanel(uint chapterID, uint episodeID) {
            for(int i = 0, l = chapterDataList.Count; i < l; i++) {
                var rt = chapterPanelList[i].GetComponent<RectTransform>();
                rt.offsetMin = new Vector2(rt.offsetMin.x, 0);
                rt.offsetMax = new Vector2(rt.offsetMax.x, 0);
                TweenX.Add(chapterPanelList[i].gameObject, 0.001f, i * Constant.SCREEN_BASE_WIDTH + Constant.SCREEN_BASE_WIDTH / 2);
                if(chapterDataList[i].ID == chapterID) {
                    selectedChapterIndex = i;
                }
            }
            carouselController.listNum = chapterDataList.Count;
            carouselController.Init(selectedChapterIndex);
        }

        public int GetChapterIndex() {
            return selectedChapterIndex;
        }

        public int GetEpisodeIndex() {
            return selectedEpisodeIndex;
        }

        public void SetChapterAndEpisodeIndex(int chapterIndex, int episodeIndex) {
            selectedChapterIndex = chapterIndex;
            selectedEpisodeIndex = episodeIndex;
            chapterPanelList[selectedChapterIndex].SetSelectedEpisodeBtn(selectedEpisodeIndex);
        }

        public void OpenStorySelect() {
            gameObject.SetActive(true);
        }

        public void CloseStorySelect() {
            gameObject.SetActive(false);
        }
    }
}
