using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public abstract class AbstractRangeAttack : AbstractRangeAction
    {
        protected Attackable _attackable;
        protected abstract float? _damageCoefficient { get; }

        public AbstractRangeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            // コストはたいしてかからないので、パラメータの最新化を優先して、ここで毎回インスタンス化する。
            _attackable = new Attackable(
                DamageType, _damageMagicalAttribute,
                _targetStateId,
                _attributeId, _getLevelDrivenAttributeParam(),
                _hitCharacterEffectID,
                _getDamageBoost(),
                _damageCoefficient
            );
        }

        protected int _getDamageBoost()
        {
            return (int)_getLevelDrivenParam1();
        }
    }
}
