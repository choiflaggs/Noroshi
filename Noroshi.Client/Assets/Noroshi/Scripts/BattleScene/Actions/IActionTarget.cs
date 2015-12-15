using UniRx;
using Noroshi.Core.Game.Action;
using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionTarget
    {
        IActionTagSet TagSet { get; }
        void ReceiveActionEvent(ActionEvent actionEvent);

        /// レベル。
        ushort Level { get; }
        /// 最大 HP。
        uint MaxHP { get; }
        /// 現 HP。
        uint CurrentHP { get; }
        /// 最大エネルギー。
        ushort MaxEnergy { get; }
        /// 物理防御力。
        uint Armor { get; }
        /// 魔法防御力。
        uint MagicRegistance { get; }
        /// 回避。
        byte Dodge { get; }

        bool IsDead { get; }
        bool IsTargetable { get; }

        Force Force { get; }

        Grid.GridPosition? GetGridPosition();

        IObservable<BulletHitEvent> GetOnHitObservable();

        void AddStatusBoosterFactor   (IStatusBoostFactor factor);
        void RemoveStatusBoosterFactor(IStatusBoostFactor factor);
        void AddStateTransitionBlockerFactor   (StateTransitionBlocker.Factor factor);
        void RemoveStateTransitionBlockerFactor(StateTransitionBlocker.Factor factor);
        void AddStatusBreakerFactor   (StatusForceSetter.Factor factor);
        void RemoveStatusBreakerFactor(StatusForceSetter.Factor factor);

        bool TryToTransitToStop();
        bool TryToTransitFromStop();
        void SetCurrentForceReverse();
        void SetCurrentForceOriginal();

        void SetSpeed(float speed);
        IObservable<ShadowCharacter> MakeShadow(IShadow shadow, ushort? level, ushort? actionLevel2, ushort? actionLevel3, ushort initialHorizontalIndex);

        IActionTargetView GetActionTargetView();
    }
}