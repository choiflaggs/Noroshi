using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Story;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.UI {
    public class StoryEpisode : MonoBehaviour {
        [SerializeField] int characterID;
        [SerializeField] GameObject guestCharacter1;
        [SerializeField] GameObject guestCharacter2;
        [SerializeField] BtnStage[] btnStageList;
        [SerializeField] GameObject[] roadObjList;
        [SerializeField] GameObject guideArrowPref;

        public Subject<StageInfo.StageData> OnStageClicked = new Subject<StageInfo.StageData>();

        private List<StageInfo.StageData> stageDataList = new List<StageInfo.StageData>();
        private GameObject character;
        private GameObject guideArrow;
        private int crntPosition = 0;
        private int targetPosition = 0;
        private List<Vector2> roadList = new List<Vector2>();
        private SkeletonAnimation skeleton;

        private void Init() {
            if(characterID > 0) {
                character = Instantiate(
                    Resources.Load("UICharacter/" + characterID + "/Character")
                ) as GameObject;
                skeleton = character.GetComponent<SkeletonAnimation>();
                character.transform.SetParent(transform);
                character.transform.localScale = new Vector3(-20, 20, 20);
                character.GetComponent<MeshRenderer>().sortingOrder = 2;
            }
            guideArrow = Instantiate(guideArrowPref) as GameObject;
            guideArrow.transform.SetParent(transform);
            guideArrow.transform.localScale = Vector3.one;
            for(int i = 0, l = btnStageList.Length; i < l; i++) {
                var n = i;
                btnStageList[n].OnClickedBtn.Subscribe(index => {
                    OnStageClicked.OnNext(stageDataList[index]);
                    SetGuideArrowPosition(index);
                    if(characterID > 0 && n != targetPosition) {
                        StartMoveCharacter(n);
                    }
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
                roadList.Add(btnStageList[i].transform.localPosition);
            }
        }

        private void StartMoveCharacter(int target) {
            if(target == crntPosition && skeleton.AnimationName != Constant.ANIM_RUN) {return;}
            var dir = target - crntPosition > 0 ? 1 : -1;
            if(skeleton.AnimationName != Constant.ANIM_RUN) {
                skeleton.state.SetAnimation(0, Constant.ANIM_RUN, true);
            }
            if(dir > 0) {
                if(skeleton.Skeleton.flipX) {
                    if(skeleton.AnimationName == Constant.ANIM_RUN) {
                        crntPosition -= dir;
                    }
                }
            } else {
                if((!skeleton.Skeleton.flipX && target < targetPosition) ||
                   (target > targetPosition && crntPosition <= target)) {
                    if(skeleton.AnimationName == Constant.ANIM_RUN) {
                        crntPosition -= dir;
                    }
                }
            }
            targetPosition = target;
            MoveCharacter(dir);
        }

        private void MoveCharacter(int direction) {
            var len = Vector2.Distance(character.transform.localPosition, roadList[crntPosition + direction]);
            if(character.transform.localPosition.x - roadList[crntPosition + direction].x > 0) {
                skeleton.Skeleton.flipX = true;
            } else {
                skeleton.Skeleton.flipX = false;
            }
            TweenXY.Add(character, len / 200f, roadList[crntPosition + direction]).Then(() => {
                crntPosition += direction;
                if(crntPosition != targetPosition) {
                    MoveCharacter(direction);
                } else {
                    skeleton.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                    skeleton.Skeleton.flipX = false;
                }
            });
        }

        private void SetGuideArrowPosition(int index) {
            var position = roadList[index];
            position.y -= 70;
            guideArrow.transform.localPosition = position;
        }

        private StageInfo.StageData ArrangeData(StoryStage data, int rank, int chapterIndex, int episodeIndex, int stageIndex) {
            var gcrm = GlobalContainer.RepositoryManager;
            var stageData = new StageInfo.StageData();
            stageData.Name = GlobalContainer.LocalizationManager.GetText(data.TextKey + ".Name");
            stageData.Stamina = data.Stamina;
            stageData.BattleID = (int)data.BattleID;
            stageData.StageID = (int)data.ID;
            stageData.ChapterIndex = chapterIndex;
            stageData.EpisodeIndex = episodeIndex;
            stageData.StageIndex = stageIndex;
            stageData.Rank = rank;
            stageData.RequireIDList = data.FixedCharacterIDs;
            stageData.CPUIDList = data.CpuCharacterIDs;
            var battleData = data.Battle;
            var enemyList = new List<BattleCharacter>();
            var itemList = new List<PossessionObject>();
            foreach(var enemy in battleData.Waves[battleData.Waves.Length - 1].BattleCharacters) {
                enemyList.Add(enemy);
            }
            foreach(var item in battleData.DroppablePossessionObjects) {
                itemList.Add(item);
            }
            stageData.EnemyList = enemyList.ToArray();
            stageData.ItemList = itemList.ToArray();
            return stageData;
        }

        public void SetEpisodeMap(uint episodeID, uint stageID, int chapterIndex, int episodeIndex, PlayerStoryStage[] playerStageDataList) {
            int stageLength = playerStageDataList.Length;
            Init();
            for(int i = 0, l = btnStageList.Length; i < l; i++) {
                if(i < stageLength) {
                    if(i > 0) {TweenA.Add(roadObjList[i - 1], 0.01f, 1);}
                    btnStageList[i].id = i;
                    stageDataList.Add(
                        ArrangeData(playerStageDataList[i].Stage, playerStageDataList[i].Rank, chapterIndex, episodeIndex, i)
                    );
                    stageDataList = stageDataList.OrderBy(s => s.StageID).ToList();
                    if(stageID == playerStageDataList[i].StageID || (stageID == 0 && i == 0)) {
                        if(i > 0) {
                            crntPosition = i - 1;
                            targetPosition = i;
                            character.transform.localPosition = roadList[i - 1];
                        } else {
                            crntPosition = 0;
                            targetPosition = 0;
                            character.transform.localPosition = roadList[0];
                        }
                        SetGuideArrowPosition(i);
                    }
                    if(i == stageLength - 1 && playerStageDataList[i].Rank < 1) {
                        if(playerStageDataList[i].Stage.Type != Enums.StageType.NormalStage) {
                            btnStageList[i].SetButtonState(false, true, true, playerStageDataList[i].Rank);
                        } else {
                            btnStageList[i].SetButtonState(false, false, true);
                        }
                    } else {
                        if(playerStageDataList[i].Stage.Type != Enums.StageType.NormalStage) {
                            btnStageList[i].SetButtonState(true, true, false);
                        } else {
                            btnStageList[i].SetButtonState(true, false, false);
                        }
                    }
                } else {
                    btnStageList[i].SetButtonState(false, false, false);
                }
            }
//            if(stageLength > 10) {enemy1.SetActive(false);}
//            gcrm.PlayerEpisodeRepository.Get(episodeID).Do(data => {
//                if(data != null && data.IsClearEpisode) {enemy2.SetActive(false);}
//            }).Subscribe();

            OnStageClicked.OnNext(stageDataList[targetPosition]);
            if(characterID > 0) {
                StartMoveCharacter(targetPosition);
            }
        }

        public float GetTargetPosition(int index) {
            return roadList[index].y;
        }
    }
}
