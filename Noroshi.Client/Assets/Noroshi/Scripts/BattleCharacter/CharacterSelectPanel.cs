using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UniRx;
using Noroshi.Game;

namespace Noroshi.UI {
    public class CharacterSelectPanel : MonoBehaviour, IPointerClickHandler {
        [SerializeField] GameObject iconRequire;
        [SerializeField] GameObject iconCPU;

        public Subject<uint> OnClickedPanel = new Subject<uint>();
        public Subject<bool> OnShowRequire = new Subject<bool>();

        public uint id;
        public uint playerCharacterID;
        public int position;
        public bool isDeca;

        private GameObject animChara;
        private Spine.AnimationState state;
        private bool isActive = true;
        private bool isRequire = false;
        private float charaSize = 22;
        private Vector2 startPosition = new Vector2(-750, 170);

        public void Init(CharacterStatus status, uint pcID) {
            animChara = Instantiate(
                Resources.Load("UICharacter/" + status.CharacterID + "/Character")
            ) as GameObject;

            animChara.transform.SetParent(transform);
            transform.localScale = Vector3.one;
            animChara.transform.localScale = new Vector2(-charaSize, charaSize);
            animChara.transform.localPosition = new Vector3(0, -100, 0);
            SetCharacterSkin(status.SkinLevel);
            id = status.CharacterID;
            playerCharacterID = pcID;
            position = (int)status.Position;
            isDeca = status.TagSet.IsDeca;
        }

        public void SetCharacterSkin(byte skinLevel) {
            var skeletonAnimation = animChara.GetComponent<SkeletonAnimation>();
            state = skeletonAnimation.state;
            if(skeletonAnimation.initialSkinName != "default") {
                skeletonAnimation.skeleton.SetSkin("step" + skinLevel);
                skeletonAnimation.skeleton.SetSlotsToSetupPose();
            }
        }

        public void SetCPU(uint characterID, Vector2 position) {
            animChara = Instantiate(
                Resources.Load("UICharacter/" + characterID + "/Character")
            ) as GameObject;
            state = animChara.GetComponent<SkeletonAnimation>().state;
            animChara.transform.SetParent(transform);
            transform.localScale = Vector3.one;
            animChara.transform.localScale = new Vector2(-charaSize, charaSize);
            animChara.transform.localPosition = new Vector3(0, -100, 0);
            MoveCharacter(position, 0);
            iconCPU.SetActive(true);
        }

        public void OnPointerClick(PointerEventData ped) {
            if(!isActive) {return;}
            if(isRequire) {
                OnShowRequire.OnNext(true);
                return;
            }
            isActive = false;
            OnClickedPanel.OnNext(playerCharacterID);
            Observable.Timer(TimeSpan.FromSeconds(0.4f)).Subscribe(_ => {
                isActive = true;
            });
        }

        public void MoveCharacter(Vector2 position, float velocity = 500f) {
            var len = Vector2.Distance(transform.localPosition, position);
            var duration = velocity > 0 ? len / (0.3f * len + velocity) : 0;
            gameObject.PauseTweens<TweenXY>();
            if(Mathf.Abs(len) < 1) {
                return;
            } else if(transform.localPosition.x - position.x > 0) {
                animChara.transform.localScale = new Vector2(charaSize, charaSize);
            } else {
                animChara.transform.localScale = new Vector2(-charaSize, charaSize);
            }

            if(duration > 0) {
                state.SetAnimation(0, Constant.ANIM_RUN, true);
                TweenXY.Add(gameObject, duration, position).Then(() => {
                    state.SetAnimation(0, Constant.ANIM_IDLE, true);
                    animChara.transform.localScale = new Vector2(-charaSize, charaSize);
                });
            } else {
                state.SetAnimation(0, Constant.ANIM_IDLE, true);
                transform.localPosition = position;
                animChara.transform.localScale = new Vector2(-charaSize, charaSize);
            }
        }

        public void AppearCharacter(Vector2 position, float duration) {
            transform.localPosition = startPosition;
            gameObject.SetActive(true);
            MoveCharacter(position, duration);
        }

        public void DisappearCharacter(float velocity = 500f) {
            var len = Vector2.Distance(transform.localPosition, startPosition);
            var duration = velocity > 0 ? len / (0.3f * len + velocity) : 0;
            state.SetAnimation(0, Constant.ANIM_RUN, true);
            animChara.transform.localScale = new Vector2(charaSize, charaSize);
            TweenXY.Add(gameObject, duration, startPosition).Then(() => {
                state.SetAnimation(0, Constant.ANIM_IDLE, true);
                animChara.transform.localScale = new Vector2(-charaSize, charaSize);
                InActiveCharacter();
            });
        }

        public void InActiveCharacter() {
            isRequire = false;
            iconRequire.SetActive(false);
            Destroy(GetComponent<TweenNull>());
            gameObject.SetActive(false);
        }

        public void SetRequire() {
            isRequire = true;
            iconRequire.SetActive(true);
        }

        public void ChangeOrder(int index) {
            TweenZ.Add(animChara, 0.05f, index - 10);
        }
    }
}
