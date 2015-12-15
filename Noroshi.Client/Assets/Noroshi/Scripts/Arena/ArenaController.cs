using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Enums;

namespace Noroshi.UI {
    public class ArenaController : MonoBehaviour {
        [SerializeField] ArenaBattleDeck arenaBattleDeck;
        [SerializeField] ArenaGuestContainer[] guestContainer;
        [SerializeField] Text txtMyRank;

        private bool isLoadMyInfo = false;
        private bool isLoadEnemyInfo = false;
        private uint playerID;

        private void Start() {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.ARENA);
            }
            var defaultCharacterIdList = BattleCharacterSelect.Instance.GetDefaultCharacter(SaveKeys.DefaultArenaBattleCharacter);
            arenaBattleDeck.gameObject.SetActive(true);
            foreach(var guest in guestContainer) {
                guest.OnBattle.Subscribe(id => {
                    playerID = (uint)id;
                    BattleCharacterSelect.Instance.OpenPanel(false, defaultCharacterIdList);
                });
            }

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultArenaBattleCharacter, playerCharacterIds);
                BattleScene.Bridge.Transition.TransitToPlayerBattle(playerID, playerCharacterIds);
            }).AddTo(this);

            BattleCharacterSelect.Instance.ReloadCharacterList();

            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            LoadMyInfo();
            LoadGuestInfo();
            while(!isLoadMyInfo || !isLoadEnemyInfo) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void LoadMyInfo() {
            List<uint> battleCharacterIdList = new List<uint>();
            GlobalContainer.RepositoryManager.PlayerArenaRepository.Get().Do(data => {
                var rank = data.Rank != null ? data.Rank : 99999;
                txtMyRank.text = rank.ToString();
                if(data.DeckCharacters != null) {
                    foreach(var chara in data.DeckCharacters) {
                        if(chara != null) {
                            battleCharacterIdList.Add(chara.CharacterID);
                        }
                    }
                }
                arenaBattleDeck.SetBattleCharacterIcon(battleCharacterIdList.ToArray());
                isLoadMyInfo = true;
            }).Subscribe();
        }

        private void LoadGuestInfo() {
            GlobalContainer.RepositoryManager.PlayerArenaRepository.GetRandomPlayers().Do(data => {
                foreach(var guest in guestContainer.Select((v, i) => new {v, i})) {
                    guest.v.SetGuestInfo(data[guest.i]);
                    if(guest.i == guestContainer.Length - 1) {
                        isLoadEnemyInfo = true;
                    }
                }
            }).Subscribe();
        }
    }
}
