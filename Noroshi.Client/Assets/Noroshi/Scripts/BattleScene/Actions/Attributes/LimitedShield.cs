using UniRx;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class LimitedShield : AbstractShield
    {
        AttributeHP _hp;

        public LimitedShield(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
            _hp = new AttributeHP((int)(coefficient));
        }

        public override void OnEnter(IActionTarget target)
        {
            _hp.Reset();
        }

        public override void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent)
        {
            base.OnReceiveActionEvent(target, actionEvent);
            if (_hp.Current <= 0)
            {
                _forceExit();
            }
        }
        protected override int _adjustHPDamage(int hpDamage)
        {
            var reducedHPDamage = hpDamage > _hp.Current ? _hp.Current : hpDamage;
            _hp.Damage(reducedHPDamage);
            return reducedHPDamage;
        }
        protected override bool _canSetHitCharacterEffectID()
        {
            return _hp.Current > 0;
        }

        public override void OnExit(IActionTarget target)
        {
            _hp.Clean();
        }

        public override IObservable<ChangeableValueEvent> GetOnChangeHPObservable()
        {
            return _hp.GetOnChangeObservable();
        }
        
        public override bool IsNegative { get { return false; } }
        protected override DamageType? _damageType { get { return _arg1 > 0 ? (DamageType?)_arg1 : null; } }
        protected override float _damageRatio { get { return (float)_arg2 / 100; } }
        protected override float _hpRecoveryRatio { get { return (float)_arg3 / 100; } }
        protected override float _energyRecoveryRatio { get { return (float)_arg4 / 100; } }
    }
}
