using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class BtnCharacterAvatar : MonoBehaviour {
        [SerializeField] Image imgAvatar;

        public Subject<uint> OnSelectAvatar = new Subject<uint>();

        private uint characterID;

        public void SetAvatarImg(uint id) {
            imgAvatar.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_1", id)
            );
            characterID = id;
        }

        public void OnSelect() {
            OnSelectAvatar.OnNext(characterID);
        } 
    }
}
