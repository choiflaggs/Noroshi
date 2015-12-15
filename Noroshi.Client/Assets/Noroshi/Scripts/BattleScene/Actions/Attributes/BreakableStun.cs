namespace Noroshi.BattleScene.Actions.Attributes
{
    public class BreakableStun : AbstractAttribute
    {
        
        public override bool IsNegative { get { return false; } }

        int? _maxHitCount { get { return _arg1 > 0 ? (int?)_arg1 : null; } }
        float _breakDamage { get { return (float)_arg2 / 100; } }
        float? _exitDamageRatio { get { return _arg3 > 0 ? (float?)_arg3 / 100 : null; } }

        int _hitCount = 0;
        int _accumulationDamge;

        public BreakableStun(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnEnter(IActionTarget target)
        {
            _hitCount = 0;
            if (_exitDamageRatio.HasValue)
            {
                _accumulationDamge = (int)(target.MaxHP * _exitDamageRatio.Value);
            }
            target.TryToTransitToStop();
        }

        public override void OnExit(IActionTarget target)
        {
            _accumulationDamge = 0;
            target.TryToTransitFromStop();
        }

        public override void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent)
        {
            _reciveActionEventWithHitCount(target, actionEvent);
            _reciveActionEventWithDamageRatio(target, actionEvent);
        }

        void _reciveActionEventWithHitCount(IActionTarget target, ActionEvent actionEvent)
        {
            if (_maxHitCount.HasValue)
            {
                if (actionEvent.DamageType.HasValue && actionEvent.DamageType.Value == DamageType.Physical)
                {
                    _hitCount++;
                    if (_hitCount >= _maxHitCount.Value)
                    {
                        target.ReceiveActionEvent(_buildActionEvent(target));
                        _forceExit();
                    }
                }
            }
        }

        void _reciveActionEventWithDamageRatio(IActionTarget target, ActionEvent actionEvent)
        {
            if (_exitDamageRatio.HasValue && actionEvent.HPDamage.HasValue)
            {
                _accumulationDamge -= actionEvent.HPDamage.Value;
                if (_accumulationDamge <= 0)
                {
                    _forceExit();
                }
            }
        }

        ActionEvent _buildActionEvent(IActionTarget target)
        {
            var actionEvent = new ActionEvent(null, target);
            actionEvent.SetHPDamage((int)(target.MaxHP * _breakDamage));
            return actionEvent;
        }
    }
}
