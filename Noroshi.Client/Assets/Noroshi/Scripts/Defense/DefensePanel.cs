using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Noroshi.UI {
    public class DefensePanel : BtnCommon {

        public void SetDefensePanel(uint id) {
            var animChara = Instantiate(
                Resources.Load("UICharacter/" + id + "/Character")
            ) as GameObject;
            var skeletonAnimation = animChara.GetComponent<SkeletonAnimation>();

            animChara.GetComponent<MeshRenderer>().sortingOrder = 1;
            if(skeletonAnimation.initialSkinName != "default") {
                skeletonAnimation.skeleton.SetSkin("step" + 3);
                skeletonAnimation.skeleton.SetSlotsToSetupPose();
            }
            animChara.transform.SetParent(transform);
            transform.localScale = Vector3.one;
            animChara.transform.localScale = new Vector3(20, 20, 20);
            animChara.transform.localPosition = new Vector3(0, -70, 0);
        }
    }
}
