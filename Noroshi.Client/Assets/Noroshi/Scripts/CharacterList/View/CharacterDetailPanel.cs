using UnityEngine;
using System.Collections.Generic;
using Noroshi.Game;
using UniLinq;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class CharacterDetailPanel : MonoBehaviour {
        [SerializeField] BtnCommon[] tabBtnList;
        [SerializeField] BtnCommon btnZoom;
        [SerializeField] GameObject tabBtnContainer;
        [SerializeField] GameObject tabContentContainer;
        [SerializeField] GameObject[] tabContentList;
        [SerializeField] GameObject abilityContainer;
        [SerializeField] GameObject charaImageContainer;
        [SerializeField] Text txtName;
        [SerializeField] Text txtHp;
        [SerializeField] Text txtPower;
        [SerializeField] Text txtLevel;
        [SerializeField] GameObject expBar;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon[] btnSwitch;
        [SerializeField] BtnCommon btnExpUp;
        [SerializeField] ExpUpPanel expUpPanel;
        [SerializeField] CharacterSlider characterSlider;

        public Subject<int> OnSwitchDetail = new Subject<int>();
        public Subject<CharacterPanel.CharaData> OnChangeExp = new Subject<CharacterPanel.CharaData>();
        public Subject<bool> OnCloseDetail = new Subject<bool>();

        private CharacterPanel.CharaData charaData;

        private bool isZoom = false;
        private Dictionary<string, GameObject> charaImageList = new Dictionary<string, GameObject>();
        private int tempLv;
        private int needExp;
        private float expBarWidth;
        private int animLoopNum = 0;
        private float characterSize = 35;
        private GameObject comeCharacter;
        private GameObject leaveCharacter;
        private float comePoint = -1000;
        private float leavePoint;
        private float comePrevPoint;
        private float leavePrevPoint;
        private float comeCharacterOffsetY = 0;
        private float leaveCharacterOffsetY = 0;
        private SkeletonAnimation skeletonAnimation;
        private SkeletonAnimation leaveSkeletonAnimation;
        private bool isSliding = false;
        private string[] anims = new string[] {
            Constant.ANIM_ATTACK, Constant.ANIM_WALK, Constant.ANIM_RUN, Constant.ANIM_WIN,
            Constant.ANIM_A_SKILL, Constant.ANIM_P_SKILL1, Constant.ANIM_P_SKILL2
        };

        public void Init() {
            foreach(var tab in tabBtnList) {
                tab.OnClickedBtn.Subscribe(id => {
                    OpenTab(id);
                });
                tab.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnZoom.OnClickedBtn.Subscribe(_ => {
                isZoom = !isZoom;
                SwitchZoom();
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                UILoading.Instance.RemoveQuery(QueryKeys.SelectedCharacter);
                CloseCharacterDetail();
            });
            btnClose.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            foreach(var btn in btnSwitch) {
                btn.OnClickedBtn.Subscribe(direction => {
                    if(isSliding) {return;}
                    isSliding = true;
                    comePoint = 0;
                    if(direction < 0) {
                        SlideCharacter(1);
                    } else {
                        SlideCharacter(-1);
                    }
                });
                btn.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            if(PlayerInfo.Instance.GetTutorialStep() >= Noroshi.Core.Game.Player.TutorialStep.ClearStoryStage6) {
                btnExpUp.OnClickedBtn.Subscribe(_ => {
                    expUpPanel.OpenExpUpPanel(charaData.characterID);
                    if((charaData.lv > PlayerInfo.Instance.PlayerLevel) ||
                        (charaData.lv == PlayerInfo.Instance.PlayerLevel && charaData.exp >= needExp - 1)) {
                        expUpPanel.SetEnableExpUpPanel(false);
                    } else {
                        expUpPanel.SetEnableExpUpPanel(true);
                    }
                });
                btnExpUp.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            expUpPanel.Init();
            expUpPanel.OnRaiseExp.Subscribe(value => {
                charaData.exp += value;
                tempLv = charaData.lv;
                CheckLevelUp();
            });

            characterSlider.OnTapCharacter.Subscribe(vec2 => {
                var diffX = vec2.x - charaImageContainer.transform.localPosition.x - Constant.SCREEN_BASE_WIDTH / 2;
                var diffY = vec2.y - charaImageContainer.transform.localPosition.y - Constant.SCREEN_BASE_HEIGHT / 2;
                if(diffX > -120 && diffX < 120 && diffY > 50 && diffY < 370) {
                    PlayCharacterAnimation();
                }
            });

            characterSlider.OnDragCharacter.Subscribe(posX => {
                if(isSliding) {return;}
                comePoint = posX;
                MoveCharacter(comeCharacter, comePoint, true, comeCharacterOffsetY, skeletonAnimation);
            });

            characterSlider.OnSlideStart.Subscribe(value => {
                comePrevPoint = comePoint;
            });

            characterSlider.OnSlideEnd.Subscribe(value => {
                if(isSliding) {return;}
                if(value < -120) {
                    SlideCharacter(1);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                } else if(value > 120) {
                    SlideCharacter(-1);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                } else {
                    DOTween.To(() => comePoint, (a) => comePoint = a, 0, 0.3f)
                        .OnUpdate(() => {MoveCharacter(comeCharacter, comePoint, true, comeCharacterOffsetY, skeletonAnimation);})
                        .OnComplete(() => {
                            comeCharacter.transform.localScale = new Vector2(-characterSize, characterSize);
                        });
                }
            });

            expBarWidth = expBar.GetComponent<RectTransform>().sizeDelta.x;

            expUpPanel.OnSendExp.Subscribe(SendUseDrug);
            btnZoom.SetSelect(false);
            OpenTab(0);
        }

        private void OpenTab(int index) {
            for(int i = 0, l = tabContentList.Length; i < l; i++) {
                if(i == index) {
                    tabBtnList[i].transform.localScale = Vector3.one;
                    tabBtnList[i].SetSelect(true);
                    TweenA.Add(tabBtnList[i].gameObject, 0.01f, 1);
                    tabContentList[i].SetActive(true);
                } else {
                    tabBtnList[i].transform.localScale = Vector3.one * 0.9f;
                    tabBtnList[i].SetSelect(false);
                    TweenA.Add(tabBtnList[i].gameObject, 0.01f, 0.7f);
                    tabContentList[i].SetActive(false);
                }
            }
        }

        private void SwitchZoom() {
            btnZoom.SetSelect(isZoom);
            PlayerInfo.Instance.gameObject.SetActive(!isZoom);
            characterSlider.gameObject.SetActive(!isZoom);
            tabBtnContainer.SetActive(!isZoom);
            tabContentContainer.SetActive(!isZoom);
            abilityContainer.SetActive(isZoom);
        }

        private void CheckLevelUp() {
            GlobalContainer.RepositoryManager.LevelMasterRepository.GetCharacterLevel((ushort)charaData.lv).Do(v => {
                needExp = (int)v.Exp;
                if(charaData.exp >= needExp) {
                    if(charaData.lv >= PlayerInfo.Instance.PlayerLevel) {
                        expUpPanel.SetEnableExpUpPanel(false);
                        charaData.exp = needExp - 1;
                        UpdateExpBar();
                    } else {
                        charaData.exp -= needExp;
                        charaData.lv++;
                        PlayCharacterAnimation(3);
                        CheckLevelUp();
                        SoundController.Instance.PlaySE(SoundController.SEKeys.LEVEL_UP);
                    }
                } else {
                    UpdateExpBar();
                }
            }).Subscribe();
        }

        private void UpdateExpBar() {
            expBar.transform.DOKill();
            if(charaData.lv - tempLv > 0) {
                expBar.transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.Linear).OnComplete(() => {
                    tempLv++;
                    txtLevel.text = tempLv.ToString();
                    expBar.transform.localPosition = new Vector3(-expBarWidth, 0, 0);
                    UpdateExpBar();
                });
            } else {
                var xx = expBarWidth - expBarWidth * (float)charaData.exp / (float)needExp;
                txtLevel.text = charaData.lv.ToString();
                expBar.transform.DOLocalMoveX(-xx, 0.2f).SetEase(Ease.Linear);
            }
        }

        private void SendUseDrug(Dictionary<string, int> data) {
            GlobalContainer.RepositoryManager.PlayerDrugRepository.UseDrugWithCharacter(
                (uint)data["itemId"], charaData.playerCharacterID, (ushort)data["useNum"]
            ).Do(_ => {
                OnChangeExp.OnNext(charaData);
            }).Subscribe();
        }

        private void SlideCharacter(int direction) {
            isSliding = true;
            leavePoint = leavePrevPoint = comePoint;
            leaveCharacter = comeCharacter;
            leaveCharacterOffsetY = comeCharacterOffsetY;
            leaveSkeletonAnimation = skeletonAnimation;
            leaveSkeletonAnimation.state.SetAnimation(0, Constant.ANIM_RUN, true);
            DOTween.To(() => leavePoint, (a) => leavePoint = a, -1000 * direction, 0.4f)
                .SetEase(Ease.Linear)
                .OnUpdate(() => {MoveCharacter(leaveCharacter, leavePoint, false, leaveCharacterOffsetY, leaveSkeletonAnimation);})
                .OnComplete(() => {
                    leaveSkeletonAnimation.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                    TweenNull.Add(leaveCharacter, 0.15f).Then(() => {leaveCharacter.SetActive(false);});
                });
            comePoint = 1000 * direction;
            comePrevPoint = comePoint;
            OnSwitchDetail.OnNext(1 * direction);
        }

        private void MoveCharacter(GameObject chara, float crntPos, bool isCome, float offsetY, SkeletonAnimation anim) {
            var posY = 0.00015f * crntPos * crntPos + 50 + offsetY;
            var ratio = crntPos * 0.015f;
            var prevPos = isCome ? comePrevPoint : leavePrevPoint;
            var c = crntPos > 0 ? 1 - crntPos * 0.001f : 1 + crntPos * 0.001f;
            if(c < 0) {c = 0;}
            chara.transform.localPosition = new Vector2(crntPos, posY);
            if(crntPos != prevPos && Mathf.Abs(crntPos) > 5) {
                if(crntPos - prevPos <= 0) {
                    if(ratio < 0) {
                        chara.transform.localScale = new Vector2(characterSize + ratio, characterSize + ratio);
                    } else {
                        chara.transform.localScale = new Vector2(characterSize - ratio, characterSize - ratio);
                    }
                } else {
                    if(ratio < 0) {
                        chara.transform.localScale = new Vector2(-characterSize - ratio, characterSize + ratio);
                    } else {
                        chara.transform.localScale = new Vector2(-characterSize + ratio, characterSize - ratio);
                    }
                }
            }
            anim.skeleton.SetColor(new Color(c, c, c));
            if(isCome) {
                comePrevPoint = crntPos;
            } else {
                leavePrevPoint = crntPos;
            }
        }

        private void OnCompleteSpineAnim(Spine.AnimationState state , int trackIndex , int loopCount) {
            if(state.ToString() == Constant.ANIM_RUN || state.ToString() == Constant.ANIM_WALK) {
                if(animLoopNum > 2) {
                    skeletonAnimation.state.Complete -= OnCompleteSpineAnim;
                    skeletonAnimation.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                }
            } else {
                skeletonAnimation.state.Complete -= OnCompleteSpineAnim;
                skeletonAnimation.state.SetAnimation(0, Constant.ANIM_IDLE, true);
            }
            animLoopNum++;
        }

        public void CreateCharaImage(CharacterStatus status) {
            var charaImg = Instantiate(
                Resources.Load("UICharacter/" + status.CharacterID + "/Character")
            ) as GameObject;
            var sa = charaImg.GetComponent<SkeletonAnimation>();

            if(sa.initialSkinName != "default") {
                sa.skeleton.SetSkin("step" + status.SkinLevel);
                sa.skeleton.SetSlotsToSetupPose();
            }
            charaImg.transform.SetParent(charaImageContainer.transform);
            charaImg.transform.localScale = new Vector2(-characterSize, characterSize);
            charaImg.transform.localPosition = new Vector2(-1000, 0);
            charaImageList["chara" + status.CharacterID] = charaImg;
            TweenNull.Add(charaImg, 0.15f).Then(() => {
                charaImg.SetActive(false);
            });
        }
        
        public void PlayCharacterAnimation(int index = -1) {
            var n = index < 0 ? UnityEngine.Random.Range(0, anims.Length) : index;
            animLoopNum = 0;
            skeletonAnimation.state.Complete += OnCompleteSpineAnim;
            skeletonAnimation.state.SetAnimation(0, anims[n], true);
            SoundController.Instance.PlayVoice(charaData.characterID, anims[n]);
        }

        public void SetCharacterSkin(int evolutionLevel) {
            var sa = charaImageList["chara" + charaData.characterID].GetComponent<SkeletonAnimation>();
            int skinLevel = 1;
            if (charaData.isDeca) {
                skinLevel = evolutionLevel;
            } else {
                skinLevel = evolutionLevel < 3 ? 1 : evolutionLevel < 5 ? 2 : 3;
            }
            if(sa.initialSkinName != "default") {
                sa.skeleton.SetSkin("step" + skinLevel);
                sa.skeleton.SetSlotsToSetupPose();
            }
        }

        public void OpenCharacterDetail(CharacterPanel.CharaData data) {
            var gcrm = GlobalContainer.RepositoryManager;
            var charaImg = charaImageList["chara" + data.characterID];
            var mesh = charaImg.GetComponent<MeshRenderer>();

            charaData = data;

            txtName.text = charaData.name;
            txtLevel.text = charaData.lv.ToString();
            txtHp.text = charaData.hp.ToString();
            txtPower.text = charaData.power.ToString();

            gcrm.LevelMasterRepository.GetCharacterLevel((ushort)data.lv).Do(v => {
                needExp = (int)v.Exp;
                var xx = expBarWidth - expBarWidth * (float)data.exp / (float)needExp;
                expBar.transform.localPosition = new Vector3(-xx, 0, 0);
            }).Subscribe();

            gameObject.SetActive(true);
            gameObject.PauseTweens();
            isSliding = true;
            TweenA.Add(gameObject, 0.15f, 1).Then(() => {
                if(!charaImg.activeSelf) {
                    charaImg.transform.localScale = new Vector2(-characterSize, characterSize);
                    comeCharacter = charaImg;
                    skeletonAnimation = charaImg.GetComponent<SkeletonAnimation>();
                    skeletonAnimation.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                    if(mesh.bounds.size.y > 4.0f) {
                        comeCharacterOffsetY = 0;
                    } else {
                        comeCharacterOffsetY = -32 * mesh.bounds.size.y + 130;
                    }
                    charaImg.transform.localPosition = new Vector2(-1000, 0);
                    charaImg.SetActive(true);
                    skeletonAnimation.state.SetAnimation(0, Constant.ANIM_RUN, true);
                    DOTween.To(() => comePoint, (a) => comePoint = a, 0, 0.8f)
                        .SetEase(Ease.Linear)
                        .OnUpdate(() => {MoveCharacter(comeCharacter, comePoint, true, comeCharacterOffsetY, skeletonAnimation);})
                        .OnComplete(() => {
                            isSliding = false;
                            skeletonAnimation.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                            charaImg.transform.localScale = new Vector2(-characterSize, characterSize);
                        });
                } else {
                    isSliding = false;
                }
            });
        }

        public void CloseCharacterDetail() {
            OnCloseDetail.OnNext(true);
            if(comeCharacter != null) {comeCharacter.SetActive(false);}
            DOTween.KillAll();
            TweenA.Add(gameObject, 0.15f, 0).Then(() => {
                comePoint = comePrevPoint = -1000;
                leavePoint = leavePrevPoint = -1000;
                isZoom = false;
                SwitchZoom();
                OpenTab(0);
                gameObject.SetActive(false);
            });
        }
    }
}