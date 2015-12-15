using UniRx;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public interface IAttribute
    {
        /// 重複チェック用 ID。同じ ID を持つものは二重かけできない（0 はチェックスキップ）。
        uint? GroupID { get; }
        /// 生存期間。
        int? Lifetime { get; }
        /// ネガティブ状態異常かどうか
        bool IsNegative { get; }
        /// 効果発動時の処理
        void OnEnter(IActionTarget target);
        /// 効果終了時の処理
        void OnExit(IActionTarget target);
        /// アクションイベントを受け取った際の処理
        void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent);
        uint CharacterEffectID { get; }
        bool CharacterEffectOncePlay { get; }
        IObservable<IAttribute> GetOnForceExit();
        IObservable<ChangeableValueEvent> GetOnChangeHPObservable();
    }
}