using Noroshi.BattleScene.Actions.Roles;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public class GiveAttribute : AbstractNoRangeAction
    {
        public GiveAttribute(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        float             _shadowNumBonus   { get { return (float)_arg3 / 100; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg4 > 0 ? (ActionEffectType?)_arg4 : null; } }

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