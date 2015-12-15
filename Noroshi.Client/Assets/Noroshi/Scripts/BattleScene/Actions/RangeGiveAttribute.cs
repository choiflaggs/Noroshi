using Noroshi.BattleScene.Actions.Roles;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public class RangeGiveAttribute : AbstractRangeAction
    {
        public RangeGiveAttribute(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        float             _shadowNumBonus   { get { return (float)_arg4 / 100; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg5 > 0 ? (ActionEffectType?)_arg5 : null; } }
        
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            foreach (var target in targets)
            {
                var actionEvent = new ActionEvent(executor, target);
                actionEvent.SetAttribute(_attributeId.Value, _getAttributeCoefficient(executor));
                target.ReceiveActionEvent(actionEvent);
            }
            _tryCameraShake();
        }
        
        float _getAttributeCoefficient(IActionExecutor executor)
        {
            return _getLevelDrivenAttributeParam() * (1 + _shadowNumBonus * executor.GetShadowNum());
        }
    }
}