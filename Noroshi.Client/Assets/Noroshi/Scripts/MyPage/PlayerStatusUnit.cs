using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.UI {
    public class PlayerStatusUnit : MonoBehaviour {
        [SerializeField] Image imgPlayerAvatar;
        [SerializeField] BtnCommon btnOpenPlayerStatus;
        [SerializeField] BtnCommon btnPlayerStatusOverlay;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon btnChangeAvatar;
        [SerializeField] GameObject playerStatusPanel;
        [SerializeField] GameObject avatarListContainer;
        [SerializeField] GameObject avatarListWrapper;
        [SerializeField] BtnCharacterAvatar btnCharacterAvatar;
        [SerializeField] Text txtFaceTeamLevel;
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtTeamLevel;
        [SerializeField] Text txtCurrentTeamExp;
        [SerializeField] Text txtNeedTeamExp;
        [SerializeField] Text txtMaxHeroLevel;
        [SerializeField] Text txtAccountId;
        [SerializeField] Text txtMaxTeamLevel;
        [SerializeField] Text txtGuildName;
        [SerializeField] Text txtGuildID;

        private void Start() {
            btnOpenPlayerStatus.OnClickedBtn.Subscribe(_ => {
                btnPlayerStatusOverlay.gameObject.SetActive(true);
                playerStatusPanel.gameObject.SetActive(true);
                TweenA.Add(playerStatusPanel, 0.1f, 1).From(0);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnChangeAvatar.OnClickedBtn.Subscribe(_ => {
                avatarListContainer.SetActive(true);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                ClosePlayerStatus();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnPlayerStatusOverlay.OnClickedBtn.Subscribe(_ => {
                ClosePlayerStatus();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        private void SetCharacterAvatarList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(charaList => {
                foreach(var chara in charaList) {
                    var btnAvatar = Instantiate(btnCharacterAvatar);
                    btnAvatar.SetAvatarImg(chara.CharacterID);
                    btnAvatar.transform.SetParent(avatarListWrapper.transform);
                    btnAvatar.transform.localScale = Vector3.one;
                    btnAvatar.OnSelectAvatar.Subscribe(SetPlayerAvatar);
                }
            }).Subscribe();
        }

        private void SetPlayerAvatar(uint id) {
            var gcrm = GlobalContainer.RepositoryManager;
//            gcrm.PlayerStatusRepository.ChangeAvaterCharacterID((ushort)id).Subscribe();
            imgPlayerAvatar.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_1", id)
            );
            avatarListContainer.SetActive(false);
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void ClosePlayerStatus() {
            TweenA.Add(playerStatusPanel, 0.1f, 0).Then(() => {
                playerStatusPanel.gameObject.SetActive(false);
                btnPlayerStatusOverlay.gameObject.SetActive(false);
            });
        }

        public void SetPlayerStatus(PlayerStatus playerStatus) {
            GlobalContainer.RepositoryManager.LevelMasterRepository.GetPlayerLevel(playerStatus.Level).Do(data => {
                txtFaceTeamLevel.text = playerStatus.Level.ToString();
                txtTeamLevel.text = playerStatus.Level.ToString();
                txtPlayerName.text = playerStatus.Name;
                txtCurrentTeamExp.text = (data.Exp - playerStatus.ExpInLevel).ToString();
                txtNeedTeamExp.text = data.Exp.ToString();
                txtMaxHeroLevel.text = playerStatus.Level.ToString();
                txtAccountId.text = playerStatus.PlayerID.ToString();
                txtMaxTeamLevel.text = "99";
            }).Subscribe();
            SetCharacterAvatarList();
        }
    }
}
