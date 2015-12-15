using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class StealFixedDamage : AbstractNoRangeAction
    {
        int _executeNum;
        Dictionary<IActionTarget, int> _damages = new Dictionary<IActionTarget, int>();
        IActionRelationView _view;
        public StealFixedDamage(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg3 > 0 ? (ActionEffectType?)_arg3 : null; } }

        public override IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory)
        {
            if (_shouldUseActionView())
            {
                return factory.BuildActionRelationView(executor.CharacterID).Select(v =>
                {
                    _view = v;
                    _view.Disappear();
                    return (IAction)this;
                });
            }
            return base.LoadAssets(executor, factory);
        }
        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _executeNum = 0;
            _damages.Clear();
        }
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            ++_executeNum;
            // ダメージ与えるタイミング
            if (_damages.Count() == 0)
            {
                foreach (var target in _noRangeSearchable.FilterTargets(actionTargetFinder, targets))
                {
                    var actionEvent = new ActionEvent(executor, target);
                    actionEvent.SetHPDamage(_getDamage());
                    if (_hitCharacterEffectID.HasValue) actionEvent.SetHitCharacterEffectID(_hitCharacterEffectID.Value);
                    target.ReceiveActionEvent(actionEvent);
                    _damages.Add(target, _getDamage());
                    // TODO : 複数対応
                    if (_shouldUseActionView()) _view.Appear(executor.GetViewAsActionExecutorView(), target.GetActionTargetView(), _getDuration());
                }
            }
            // ダメージを吸収するタイミング
            else
            {
                var target = actionTargetFinder.GetExecutorAsTarget(executor);
                foreach (var kv in _damages)
                {
                    var actionEvent = new ActionEvent(executor, target);
                    actionEvent.SetHPDamage(-kv.Value);
                    target.ReceiveActionEvent(actionEvent);
                    // TODO : 複数対応
                    if (_shouldUseActionView()) _view.Disappear();
                }
            }
        }

        int _getDamage()
        {
            return (int)_getLevelDrivenParam1();
        }
        float _getDuration()
        {
            return _animation.TriggerTimes[_executeNum] - _animation.TriggerTimes[_executeNum - 1];
        }

        public override void EnterTimeStop()
        {
            if (_shouldUseActionView()) _view.PauseOn();
        }
        public override void ExitTimeStop()
        {
            if (_shouldUseActionView()) _view.PauseOff();
        }

        public override void Cancel(IActionExecutor executor)
        {
            if (_shouldUseActionView()) _view.Disappear();
        }
    }
}