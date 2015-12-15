using UniRx;

namespace Noroshi.BattleScene.UI
{
    /// 各自キャラクター用パネルクラス。
    public class OwnCharacterPanelUI : UIViewModel<IOwnCharacterPanelUIView>
    {
        byte _characrerNo;
        uint _characrerId;
        int _activeActionLevel;
        uint _initialHp;
        uint _maxHp;
        int _initialEnergy;
        int _maxEnergy;
        int _skinLevel;
        Subject<byte> _onClickSubject = new Subject<byte>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public OwnCharacterPanelUI(byte characterNo, uint characterId, int activeActionLevel,
                                   uint initialHp, uint maxHp, int initialEnergy, int maxEnergy, int skinLevel)
        {
            _characrerNo = characterNo;
            _characrerId = characterId;
            _activeActionLevel = activeActionLevel;
            _initialHp = initialHp;
            _maxHp = maxHp;
            _initialEnergy = initialEnergy;
            _maxEnergy = maxEnergy;
            _skinLevel = skinLevel;
        }

        /// クリックしたらキャラクター番号がプッシュされる Observable を取得
        public IObservable<byte> GetOnClickObservable()
        {
            return _onClickSubject.AsObservable();
        }

        public void FinishActiveAction()
        {
            _uiView.FinishActiveAction();
        }

        public void EnterActiveAction()
        {
            _uiView.EnterActiveAction();
        }

        /// HP 関連表現を変更する。
        public void ChangeHP(ChangeableValueEvent hpEvent)
        {
            _uiView.ChangeHP(hpEvent);
        }

        /// エネルギー関連表現を変更する。
        public void ChangeEnergy(ChangeableValueEvent energyEvent)
        {
            _uiView.ChangeEnergy(energyEvent);
        }

        /// アクティブアクション実行可否表現を切り替える。
        public void ToggleActiveActionAvailable(bool available)
        {
            _uiView.ToggleActiveActionAvailable(available);
        }

        public void ChangeStatusBoost(CharacterStatusBoostEvent boostEvent)
        {
            _uiView.ChangeStatusBoost(boostEvent);
        }

        protected override IObservable<IOwnCharacterPanelUIView> _loadView()
        {
            // ロード直後にクリック処理を紐付ける
            return SceneContainer.GetFactory().BuildOwnCharacterPanelUIView(_characrerId, _skinLevel).Do(_onLoadView);
        }
        void _onLoadView(IOwnCharacterPanelUIView view)
        {
            view.Initialize(_activeActionLevel, _initialHp, _maxHp, _initialEnergy, _maxEnergy);
            view.GetOnClickObservable().Subscribe(_ => _onClickSubject.OnNext(_characrerNo)).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }
}
