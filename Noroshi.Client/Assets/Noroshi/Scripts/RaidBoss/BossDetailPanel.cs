using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response.RaidBoss;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.UI {
    public class BossDetailPanel : MonoBehaviour {
        public class BossBattleData {
            public uint GuildRaidBossId;
            public byte UseBP;
            public PlayerGuildRaidBoss[] DamageRanking;
            public RaidBossLog[] Logs;
            public PossessionObject[] DiscoveryRewards;
            public PossessionObject[] EntryRewards;
        }

        [SerializeField] GameObject characterContainer;
        [SerializeField] GameObject normalSign;
        [SerializeField] GameObject bigSign;
        [SerializeField] Text txtAttackerNum;
        [SerializeField] Text txtAttackerNumReward;
        [SerializeField] Text txtGivenDamage;
        [SerializeField] Text txtGivenNumReward;
        [SerializeField] BtnCommon btnRewardDetail;
        [SerializeField] Text txtBossName;
        [SerializeField] Text txtBossLevel;
        [SerializeField] Text txtCurrentHP;
        [SerializeField] Text txtMaxHP;
        [SerializeField] GameObject hpBar;
        [SerializeField] GameObject inBattleContainer;
        [SerializeField] GameObject timeUpContainer;
        [SerializeField] GameObject defeatContainer;
        [SerializeField] GameObject specialAttackContainer;
        [SerializeField] GameObject discoverBoonus;
        [SerializeField] BtnCommon[] btnBPList;
        [SerializeField] BtnCommon btnGetReward;
        [SerializeField] Text txtBattleInfo;
        [SerializeField] BtnCommon btnBattleDetail;

        public Subject<BossBattleData> OnSelectBattle = new Subject<BossBattleData>();
        public Subject<BossBattleData> OnOpenDetail = new Subject<BossBattleData>();
        public Subject<BossBattleData> OnOpenReward = new Subject<BossBattleData>();
        public Subject<uint> OnGetReward = new Subject<uint>();

        private BossBattleData bossBattleData = new BossBattleData();
        private float charaSize = 42;

        private void Start() {
            btnBattleDetail.SetEnable(false);
            Noroshi.RaidBoss.WebApiRequester.Show(bossBattleData.GuildRaidBossId).Do(data => {
                var txt = "";
                foreach(var d in data.Logs) {
                    var t = Constant.UNIX_EPOCH.AddSeconds(d.CreatedAt).ToLocalTime();
                    var hour = t.Hour < 10 ? "0" + t.Hour : t.Hour.ToString();
                    var minute = t.Minute < 10 ? "0" + t.Minute : t.Minute.ToString();
                    txt = txt + hour + ":" + minute + "  " + d.Player.Name + "   /   ";
                }
                txtBattleInfo.text = txt;
                bossBattleData.DamageRanking = data.DamageRanking;
                bossBattleData.Logs = data.Logs;
                btnBattleDetail.SetEnable(true);
            }).Subscribe();

            btnRewardDetail.OnClickedBtn.Subscribe(_ => {
                OnOpenReward.OnNext(bossBattleData);
            });
            btnBattleDetail.OnClickedBtn.Subscribe(_ => {
                OnOpenDetail.OnNext(bossBattleData);
            });
            btnBattleDetail.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnGetReward.OnClickedBtn.Subscribe(_ => {
                btnGetReward.SetEnable(false);
                OnGetReward.OnNext(bossBattleData.GuildRaidBossId);
            });
            btnGetReward.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });
        }

        public void SetBPButtonState() {
            var currentBP = PlayerInfo.Instance.CurrentBP;
            for(int i = 0, l = btnBPList.Length; i < l; i++) {
                if(i < currentBP) {
                    btnBPList[i].SetEnable(true);
                } else {
                    btnBPList[i].SetEnable(false);
                }
            }
        }

        public void SetPanel(Noroshi.Core.WebApi.Response.RaidBoss.RaidBoss bossData) {
            Debug.Log("id: " + bossData.ID + ", bossID: " + bossData.RaidBossID);
            var timespan = (DateTime.UtcNow.ToUniversalTime() - Constant.UNIX_EPOCH).TotalSeconds;
            var hpBarWidth = hpBar.GetComponent<RectTransform>().sizeDelta.x;
            var xx = hpBarWidth - hpBarWidth * (float)bossData.CurrentHP / (float)bossData.MaxHP;
//            var character = Instantiate(
//                Resources.Load("UICharacter/" + bossData.RaidBossID + "/Character")
//            ) as GameObject;
            var character = Instantiate(
                Resources.Load("UICharacter/" + 501 + "/Character")
            ) as GameObject;
            bossBattleData.GuildRaidBossId = bossData.ID;
            bossBattleData.DiscoveryRewards = bossData.DiscoveryRewards;
            bossBattleData.EntryRewards = bossData.EntryRewards;
            character.transform.SetParent(characterContainer.transform);
            character.transform.localScale = new Vector2(charaSize, charaSize);
            character.transform.localPosition = Vector3.zero;
            character.GetComponent<MeshRenderer>().sortingOrder = 1;
            txtBossName.text = bossData.TextKey;
            txtBossLevel.text = bossData.Level.ToString();
            txtCurrentHP.text = bossData.CurrentHP.ToString();
            txtMaxHP.text = bossData.MaxHP.ToString();
            txtGivenDamage.text = bossData.OwnPlayerDamage.ToString();
            hpBar.transform.localPosition = new Vector3(-xx, 0, 0);
            if((float)bossData.CurrentHP / (float)bossData.MaxHP < 0.25f) {
                txtCurrentHP.color = Constant.BAR_COLOR_ALERT;
                hpBar.GetComponent<Image>().color = Constant.BAR_COLOR_ALERT;
            } else {
                txtCurrentHP.color = Constant.BAR_COLOR_NORMAL;
                hpBar.GetComponent<Image>().color = Constant.BAR_COLOR_NORMAL;
            }
            if(bossData.Type == Noroshi.Core.Game.RaidBoss.RaidBossGroupType.Special) {
                bigSign.SetActive(true);
            } else {
                normalSign.SetActive(true);
            }
            if(bossData.IsDefeated) {
                inBattleContainer.SetActive(false);
                character.GetComponent<SkeletonAnimation>().timeScale = 0;
                character.GetComponent<SkeletonAnimation>().skeleton.SetColor(new Color(0.5f, 0.5f, 0.5f));
                defeatContainer.SetActive(true);
            } else if(bossData.EscapedAt - timespan <= 0) {
                inBattleContainer.SetActive(false);
                character.GetComponent<SkeletonAnimation>().timeScale = 0;
                timeUpContainer.SetActive(true);
            } else {
                SetBPButtonState();
                foreach(var btn in btnBPList) {
                    btn.OnClickedBtn.Subscribe(bp => {
                        bossBattleData.UseBP = (byte)bp;
                        OnSelectBattle.OnNext(bossBattleData);
                    });
                    btn.OnPlaySE.Subscribe(_ => {
                        SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
                    });
                }
            }
        }
    }
}
