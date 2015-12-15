using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class BattleDetailPanel : MonoBehaviour {
        [SerializeField] BtnCommon[] tabList;
        [SerializeField] GameObject[] contentList;
        [SerializeField] BtnCommon btnOverlay;
        [SerializeField] GameObject damageRankWrapper;
        [SerializeField] GameObject battleLogWrapper;
        [SerializeField] RankingPlayerPiece rankingPlayerPiecePref;
        [SerializeField] BattleLogPiece battleLogPiecePref;

        private List<RankingPlayerPiece> rankingPlayerPieceList = new List<RankingPlayerPiece>();
        private List<BattleLogPiece> battleLogPieceList = new List<BattleLogPiece>();

        private void Start() {
            for(int i = 0, l = tabList.Length; i < l; i++) {
                var n = i;
                tabList[i].OnClickedBtn.Subscribe(SwitchTab);
                tabList[i].OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnOverlay.OnClickedBtn.Subscribe(_ => {
                ClosePanel();
            });
            btnOverlay.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        private void SwitchTab(int index) {
            for(int i = 0, l = contentList.Length; i < l; i++) {
                var isSelect = index == i;
                tabList[i].SetSelect(isSelect);
                contentList[i].SetActive(isSelect);
            }
        }

        private void SetDamageRank(Noroshi.Core.WebApi.Response.RaidBoss.PlayerGuildRaidBoss[] damageList) {
            if(damageList.Length > rankingPlayerPieceList.Count) {
                for(int i = 0, l = damageList.Length - rankingPlayerPieceList.Count; i < l; i++) {
                    var piece = Instantiate(rankingPlayerPiecePref);
                    piece.transform.SetParent(damageRankWrapper.transform);
                    piece.transform.localScale = Vector3.one;
                    rankingPlayerPieceList.Add(piece);
                }
            }
            for(int i = 0, l = rankingPlayerPieceList.Count; i < l; i++) {
                if(i < damageList.Length) {
                    rankingPlayerPieceList[i].SetInfo(damageList[i], i);
                    rankingPlayerPieceList[i].gameObject.SetActive(true);
                } else {
                    rankingPlayerPieceList[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetBattleLog(Noroshi.Core.WebApi.Response.RaidBoss.RaidBossLog[] logList) {
            if(logList.Length > battleLogPieceList.Count) {
                for(int i = 0, l = logList.Length - battleLogPieceList.Count; i < l; i++) {
                    var piece = Instantiate(battleLogPiecePref);
                    piece.transform.SetParent(battleLogWrapper.transform);
                    piece.transform.localScale = Vector3.one;
                    battleLogPieceList.Add(piece);
                }
            }
            for(int i = 0, l = battleLogPieceList.Count; i < l; i++) {
                if(i < logList.Length) {
                    battleLogPieceList[i].SetInfo(logList[i]);
                    battleLogPieceList[i].gameObject.SetActive(true);
                } else {
                    battleLogPieceList[i].gameObject.SetActive(false);
                }
            }
        }

        public void ClosePanel() {
            TweenA.Add(gameObject, 0.15f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void OpenPanel(BossDetailPanel.BossBattleData data) {
            SetDamageRank(data.DamageRanking);
            SetBattleLog(data.Logs);
            SwitchTab(0);
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.15f, 1).From(0);
        }
    }
}
