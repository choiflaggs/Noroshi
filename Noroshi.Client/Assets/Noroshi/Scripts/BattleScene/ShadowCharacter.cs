using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene
{
    /// 分身キャラクターを扱うクラス。
    public class ShadowCharacter : Character, IShadow
    {
        /// 分身キャラクター ID。
        public readonly uint ShadowCharacterID;
        ushort? _overrideLevel;
        ushort? _overrideActionLevel2;
        ushort? _overrideActionLevel3;
        public ushort InitialHorizontalIndex;

        /// 分身キャラクター ID でインスタンス化。
        public ShadowCharacter(uint shadowCharacterId)
        {
            ShadowCharacterID = shadowCharacterId;
        }

        /// データとアセットをロードする。ロード後、View は非表示にしておく。
        public IObservable<IShadow> LoadDatasAndAssets(IActionFactory factory)
        {
            return LoadDatas().SelectMany(_ => LoadAssets(factory).Do(c => _view.SetActive(false))).Select(_ => (IShadow)this);
        }
        /// キャラクターステータスをロードする。
        protected override IObservable<CharacterStatus> _loadCharacterStatus()
        {
            return GlobalContainer.RepositoryManager.LoadCharacterStatusByShadowCharacterID(ShadowCharacterID)
            .Do(characterStatus => characterStatus.OverrideLevels(_overrideLevel, _overrideActionLevel2, _overrideActionLevel3));
        }

        /// 出現する。
        public void Appear(ushort? overrideLevel, ushort? overrideActionLevel2, ushort? overrideActionLevel3, ushort initialHorizontalIndex)
        {
            _overrideLevel = overrideLevel;
            _overrideActionLevel2 = overrideActionLevel2;
            _overrideActionLevel3 = overrideActionLevel3;
            InitialHorizontalIndex = initialHorizontalIndex;
            _view.SetActive(true);
        }

        /// 逃走。内部的には直接死亡状態遷移となる。
        public void Escape()
        {
            _tryToTransitToDead();
        }
        /// 死亡（死亡時に流れるだけで必ずしも死亡アニメーションとは限らず逃走、待機もあり）アニメーションを再生する。
        /// オーバーライドする場合は、終了時に処理を引っ掛けるので演出完了時に OnNext(this) される Observable を返し、OnCompleted() も忘れずに呼ぶこと。
        protected override IObservable<Character> _playDieAnimation()
        {
            return _view.PlayEscape().Select(_ => (Character)this);
        }
    }
}
