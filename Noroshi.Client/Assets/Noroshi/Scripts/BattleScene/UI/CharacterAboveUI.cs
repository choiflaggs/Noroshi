using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class CharacterAboveUI : UIViewModel<ICharacterAboveUIView>
    {
        CompositeDisposable _disposables = new CompositeDisposable();
        MonoBehaviours.IUIView _textUICanvas = null;

        public void SetTarget(Character character)
        {
            _uiView.SetTarget(character.GetView(), character.Force, character.CurrentHP, character.MaxHP);
            // HP 変更イベント処理紐付け
            character.GetOnHPChangeObservable().Where(e => !e.IgnoreAboveUI).Subscribe(_onHPChange).AddTo(_disposables);
            // エネルギー変更イベント処理紐付け
            character.GetOnEnergyChangeObservable().Where(e => !e.IgnoreAboveUI).Subscribe(_onEnergyChange).AddTo(_disposables);
            // 特別イベント処理紐付け
            character.GetOnSpecialEventObservable().Subscribe(_onSpecialEvent).AddTo(_disposables);
            // シールド割合変化処理紐付け
            character.GetOnChangeShieldRatioObservable().Subscribe(_onChangeShieldRatio).AddTo(_disposables);
            // ステータス増減処理紐付け
            character.GetOnChangeStatusBooster().Subscribe(_onChangeStatusBooster).AddTo(_disposables);
        }

        void _onHPChange(ChangeableValueEvent hpChangeEvent)
        {
            // HP ゲージを変更しつつ
            _updateHPGauge(hpChangeEvent.Current, hpChangeEvent.Max);
            // ダメージビュー表示
            _sendHPDifference(hpChangeEvent.Difference);
        }
        void _onEnergyChange(ChangeableValueEvent energyChangeEvent)
        {
            _sendEnergyDifference(energyChangeEvent.Difference);
        }
        void _onChangeShieldRatio(ChangeableValueEvent changeEvent)
        {
            _uiView.ChangeShieldRatio((float)changeEvent.Current / changeEvent.Max);
        }
        void _onChangeStatusBooster(CharacterStatusBoostEvent changeEvent)
        {
            // TODO : 実装
        }
        void _onSpecialEvent(SpecialEvent specialEvent)
        {
            _sendText(specialEvent.ToString());
        }

        void _sendText(string text)
        {
            SceneContainer.GetCacheManager().GetCharacterTextUIViewCache().Get()
            .SelectMany(v => v.Appear(_textUICanvas, _uiView.GetLocalPosition(), text))
            .Subscribe(_stockCharacterTextUIView);
        }
        void _sendHPDifference(int difference)
        {
            SceneContainer.GetCacheManager().GetCharacterTextUIViewCache().Get()
            .SelectMany(v => v.AppearHPDifference(_textUICanvas, _uiView.GetLocalPosition(), difference))
            .Subscribe(_stockCharacterTextUIView);
        }
        void _sendEnergyDifference(int difference)
        {
            SceneContainer.GetCacheManager().GetCharacterTextUIViewCache().Get()
            .SelectMany(v => v.AppearEnergyDifference(_textUICanvas, _uiView.GetLocalPosition(), difference))
            .Subscribe(_stockCharacterTextUIView);
        }
        void _stockCharacterTextUIView(ICharacterTextUIView view)
        {
            SceneContainer.GetCacheManager().GetCharacterTextUIViewCache().Stock(view);
        }

        public IObservable<ICharacterAboveUIView> LoadView(IUIController uiController)
        {
            _textUICanvas = uiController.GetTextUICanvas();
            return LoadView().Do(v => uiController.SetToWorldUICanvas(v));
        }

        protected override IObservable<ICharacterAboveUIView> _loadView()
        {
            return SceneContainer.GetCacheManager().GetCharacterAboveUIViewCache().Get().Do(v => v.SetActive(true));
        }

        void _updateHPGauge(int currentHP, int maxHP)
        {
            _uiView.ChangeHPRatio((float)currentHP / maxHP);
        }

        public void Stock()
        {
            SetViewActive(false);
            _uiView.Reset();
            _disposables.Clear();
            SceneContainer.GetCacheManager().GetCharacterAboveUIViewCache().Stock(_uiView);
        }
        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
}