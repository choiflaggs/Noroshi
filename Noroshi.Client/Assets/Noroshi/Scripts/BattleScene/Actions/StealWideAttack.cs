using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class StealWideAttack : AbstractWideAttack
    {
        int _executeNum;
        int _totalDamage;
        public StealWideAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected override int _attackMinRange { get { return _arg4; } }
        protected override int _attackMaxRange { get { return _arg5; } }
        protected override bool _isAsynchronous { get { return false; } }
        protected override float? _energyDamageRatio { get { return null; } }
        protected float? _hpHealRatio { get { return _arg6 > 0 ? (float?)_arg6 / 100.0f : null; } }
        protected uint? _healCharacterEffectID { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        // TODO
        // 現状WideAttack系はActionViewしか使用していないので一旦固定値で、カメラ揺れなど必要になった場合対応予定
        protected override ActionEffectType? _actionEffectType { get { return ActionEffectType.ViewOnly; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _executeNum = 0;
            _totalDamage = 0;
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _executeNum++;
            if (_executeAttributeNum.HasValue) _attackable.AttributeID = _executeAttributeNum.Value == _executeNum ? _attributeId : null;

            if (_animation.TriggerTimes.Length != _executeNum)
            {
                var actionEvents = _attack(actionTargetFinder, executor);
                foreach(var actionEvent in actionEvents)
                {
                    if (!actionEvent.HPDamage.HasValue) continue;
                    _totalDamage += actionEvent.HPDamage.Value;
                }
            }
            else
            {
                if (_hpHealRatio.HasValue)
                {
                    var target = actionTargetFinder.GetExecutorAsTarget(executor);
                    var actionEvent = new ActionEvent(executor, target);
                    actionEvent.SetHPDamage(-(int)(_totalDamage * _hpHealRatio.Value));
                    if (_healCharacterEffectID.HasValue) actionEvent.SetHitCharacterEffectID(_healCharacterEffectID.Value);
                    target.ReceiveActionEvent(actionEvent);
                }
            }
        }
    }
}