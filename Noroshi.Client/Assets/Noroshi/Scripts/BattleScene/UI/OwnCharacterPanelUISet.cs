using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    /// 各自キャラクター用パネルの集合を扱うクラス。
    public class OwnCharacterPanelUISet
    {
        Dictionary<byte, OwnCharacterPanelUI> _ownCharacterPanelUIs = new Dictionary<byte, OwnCharacterPanelUI>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public OwnCharacterPanelUISet(IEnumerable<Character> characters)
        {
            foreach (var character in characters.OrderByDescending(c => c.OrderPriority))
            {
                var ui = new OwnCharacterPanelUI(character.No, character.CharacterID, character.GetActiveActionLevel(),
                                                 character.CurrentHP, character.MaxHP, character.Energy.Current, character.MaxEnergy, character.SkinLevel);
                // キャラクターからプッシュされる情報に必要な処理を紐付ける。
                character.GetOnHPChangeObservable().Subscribe(ui.ChangeHP).AddTo(_disposables);
                character.GetOnEnergyChangeObservable().Subscribe(ui.ChangeEnergy).AddTo(_disposables);
                character.GetOnToggleActiveActionAvailable().Subscribe(ui.ToggleActiveActionAvailable).AddTo(_disposables);
                character.GetOnChangeStatusBooster().Subscribe(ui.ChangeStatusBoost).AddTo(_disposables);
                character.GetOnExitTimeStopObservable().Subscribe(_ => ui.FinishActiveAction()).AddTo(_disposables);
                character.GetOnEnterActiveActionObservable().Subscribe(_ => ui.EnterActiveAction()).AddTo(_disposables);
                _ownCharacterPanelUIs.Add(character.No, ui);
            }
        }

        public IObservable<OwnCharacterPanelUISet> LoadAssets(IUIController uiController)
        {
            return Observable.WhenAll<IOwnCharacterPanelUIView>(
                _ownCharacterPanelUIs.Values.Select(playerCharacterPanelUI => playerCharacterPanelUI.LoadView().Do(v => uiController.AddPlayerCharacterPanelUI(v)))
            )
            .Select(_ => this);
        }

        /// クリックしたらキャラクター番号がプッシュされる Observable を取得
        public IObservable<byte> GetOnClickObservable()
        {
            return _ownCharacterPanelUIs.Values.Select(ui => ui.GetOnClickObservable()).Merge();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            foreach (var ownCharacterPanelUI in _ownCharacterPanelUIs.Values)
            {
                ownCharacterPanelUI.Dispose();
            }
            _ownCharacterPanelUIs.Clear();
        }
    }
}
