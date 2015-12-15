using System;
using UniRx;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class PeriodicDamage : AbstractAttribute
    {
        IDisposable _disposable;

        public PeriodicDamage(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnEnter(IActionTarget target)
        {
            _disposable = SceneContainer.GetTimeHandler().Interval(_interval)
            .Do(t =>
            {
                target.ReceiveActionEvent(_buildActionEvent(target));
            })
            .Subscribe();
        }
        public override void OnExit(IActionTarget target)
        {
            _disposable.Dispose();
        }

        public override bool IsNegative { get { return false; } }

        int _interval  { get { return _arg1; } }
        int _multiplyCoefficient { get { return _arg2; } }

        ActionEvent _buildActionEvent(IActionTarget target)
        {
            var actionEvent = new ActionEvent(null, target);
            actionEvent.SetHPDamage((int)(_coefficient * _multiplyCoefficient));
            return actionEvent;
        }
    }
}