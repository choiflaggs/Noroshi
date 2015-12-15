using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response.Story;

namespace Noroshi.UI {
    public class BtnEpisode : BtnCommon {
        [SerializeField] Text txtEpisodeName;
        [SerializeField] Image imgEpisodeBtn;
        [SerializeField] GameObject selectFrame;

        public Subject<bool> OnClicked = new Subject<bool>();


        public void SetBtnEpisode(Noroshi.Core.WebApi.Response.Story.StoryEpisode episode, int index) {
            id = index;
            txtEpisodeName.text = GlobalContainer.LocalizationManager.GetText(episode.TextKey + ".Name");
            imgEpisodeBtn.sprite = Resources.Load<Sprite>(
                string.Format("Story/Chapter{0}/Episode/{1}", episode.ChapterID, index + 1)
            );
        }

        public void SetBtnState(bool isOpen, bool isCurrent) {
            if(isCurrent) {
                OnClickedBtn.OnNext(id);
            }
            if(isOpen) {
                isEnable = true;
                txtEpisodeName.color = Constant.TEXT_COLOR_NORMAL_WHITE;
                imgEpisodeBtn.color = Color.white;
            } else {
                isEnable = false;
                txtEpisodeName.color = new Color(0.5f, 0.5f, 0.5f);
                imgEpisodeBtn.color = new Color(0.3f, 0.3f, 0.3f);
            }
        }

        public void SetSelected(bool isSelect) {
            selectFrame.SetActive(isSelect);
        }
    }
}
