namespace Noroshi.BattleScene.Actions.Attributes
{
    public abstract class AbstractShield : AbstractAttribute
    {
        public AbstractShield(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnEnter(IActionTarget target)
        {
        }

        public override void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent)
        {
            if (_isTargetActionEvent(actionEvent))
            {
                var hpDamage = (int)(actionEvent.HPDamage.Value * _damageRatio);
                var reducedHPDamage = _adjustHPDamage(hpDamage);
                actionEvent.ReduceHPDamage(reducedHPDamage);
                // 回復
                var recoveryHp = (int)(reducedHPDamage * _hpRecoveryRatio);
                var recoveryEnergy = (int)(reducedHPDamage * _energyRecoveryRatio);
                if (recoveryHp > 0 || recoveryEnergy > 0)
                {
                    var recoveryActionEvent = new ActionEvent(null, target);
                    if (recoveryHp > 0)
                    {
                        recoveryActionEvent.SetHPDamage(-recoveryHp);
                    }
                    if (recoveryEnergy > 0)
                    {
                        recoveryActionEvent.SetEnergyDamage(-recoveryEnergy);
                    }
                    target.ReceiveActionEvent(recoveryActionEvent);
                }
                // シールド効果発動演出
                if (_receiveDamageCharacterEffectID.HasValue && _canSetHitCharacterEffectID())
                {
                    actionEvent.SetHitCharacterEffectID(_receiveDamageCharacterEffectID.Value, true);
                }
            }
        }
        protected virtual int _adjustHPDamage(int hpDamage)
        {
            return hpDamage;
        }
        protected virtual bool _canSetHitCharacterEffectID()
        {
            return true;
        }

        public override void OnExit(IActionTarget target)
        {
        }

        protected abstract DamageType? _damageType { get; }
        protected abstract float _damageRatio { get; }
        protected abstract float _hpRecoveryRatio { get; }
        protected abstract float _energyRecoveryRatio { get; }

        bool _isTargetActionEvent(ActionEvent actionEvent)
        {
            // ダメージなしはスルー
            if (!actionEvent.HPDamage.HasValue) return false;
            // 無属性ダメージはスルー
            if (!actionEvent.DamageType.HasValue) return false;
            // 対象ダメージ属性ありだが、やってきたダメージ属性が異なる場合はスルー
            if (_damageType.HasValue && actionEvent.DamageType.Value != _damageType.Value) return false;
            return true;
        }
    }
}
