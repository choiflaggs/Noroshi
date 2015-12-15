using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public abstract class AbstractMultiAction : AbstractAction
    {
        protected IAction[] _actions;

        public AbstractMultiAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        public override IObservable<IAction> LoadAdditionalDatas(Repositories.Server.ActionRepository repository)
        {
            return repository.GetMulti(_getActionIds().ToArray())
            .SelectMany(datas => {
                _actions = ActionBuilder.BuildMulti(datas);
                for (var i = 0; i < _actions.Length; i++)
                {
                    _actions[i].SetRank(Rank);
                    _actions[i].SetLevel(Level);
                }
                return Observable.WhenAll(_actions.Select(a => a.LoadAdditionalDatas(repository)));
            })
            .Select(_ => (IAction)this);
        }

        public override IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory)
        {
            return Observable.WhenAll(_actions.Select(a => {
                a.SetAnimation(_animation);
                return a.LoadAssets(executor, factory);
            })).Select(_ => (IAction)this);
        }

        public override IObservable<KeyValuePair<byte, Dictionary<byte, int>>> GetOnOverrideArgsObservable()
        {
            return _actions.Select(action => action.GetOnOverrideArgsObservable()).Merge();
        }

        public override IObservable<KeyValuePair<byte, uint>> GetOnOverrideAttributeIdsObservable()
        {
            return _actions.Select(action => action.GetOnOverrideAttributeIdsObservable()).Merge();
        }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
                for (var i = 0; i < _actions.Length; i++)
                {
                    _actions[i].PreProcess(executor, targetCandidates);
                }
        }
        public override void PostProcess(IActionExecutor executor)
        {
                for (var i = 0; i < _actions.Length; i++)
                {
                    _actions[i].PostProcess(executor);
                }
        }

        public override void EnterTimeStop()
        {
            for(var i = 0; i < _actions.Length; i++)
            {
                if (_actions[i] != null) _actions[i].EnterTimeStop();
            }
        }
        public override void ExitTimeStop()
        {
            for(var i = 0; i < _actions.Length; i++)
            {
                if (_actions[i] != null) _actions[i].ExitTimeStop();
            }
        }

        protected virtual List<uint> _getActionIds()
        {
            return (new List<uint?>{_actionId1, _actionId2, _actionId3, _actionId4, _actionId5, _actionId6, _actionId7, _actionId8}).Where(aid => aid.HasValue).Cast<uint>().ToList();
        }

        protected abstract uint? _actionId1  { get; }
        protected abstract uint? _actionId2  { get; }
        protected abstract uint? _actionId3  { get; }
        protected abstract uint? _actionId4  { get; }
        protected abstract uint? _actionId5  { get; }
        protected abstract uint? _actionId6  { get; }
        protected abstract uint? _actionId7  { get; }
        protected abstract uint? _actionId8  { get; }
    }
}