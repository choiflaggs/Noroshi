using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene.Actions
{
    public class ActionHandler
    {
        uint[] _actionIds;
        ushort[] _actionLevels;
        ActionSequence _sequence;
        IAction[] _actions;
        Dictionary<byte, IAction> _rankToNormalActionMap = new Dictionary<byte, IAction>();
        Dictionary<byte, uint> _rankToCurrentActionNum = new Dictionary<byte, uint>();
        IAction _currentAction;
        IActionTarget[] _currentTargetCandidates;
        bool _finishSuccessfully = false;
        Subject<SoundEvent> _onCommandSoundSubject = new Subject<SoundEvent>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public ActionHandler(uint[] actionIds, ushort[] actionLevels, ActionSequence sequence)
        {
            _actionIds = actionIds;
            _actionLevels = actionLevels;
            _sequence = sequence;
        }

        public IObservable<ActionHandler> LoadDatas()
        {
            return GlobalContainer.RepositoryManager.ActionRepository.GetMulti(_actionIds.Where(id => id != 0).ToArray())
            .SelectMany(datas => {
                _actions = ActionBuilder.BuildMulti(datas);
                int actionCount = 0;
                for (var i = 0; i < _actionIds.Length; i++)
                {
                    if (_actionIds[i] == 0) continue;
                    var rank = (byte)i;
                    _actions[actionCount].SetRank(rank);
                    _actions[actionCount].SetLevel(_actionLevels[i]);
                    _rankToCurrentActionNum.Add(rank, 0);
                    if (_actions[actionCount].Trigger == Trigger.Normal) _rankToNormalActionMap.Add(rank, _actions[actionCount]);
                    actionCount++;
                }
                return Observable.WhenAll(_actions.Select(a => a.LoadAdditionalDatas(GlobalContainer.RepositoryManager.ActionRepository)));
            })
            .Do(_ => 
            {
                _actions.Select(action => action.GetOnOverrideArgsObservable()).Merge().Subscribe(rankToOverrideArgs =>
                {
                    _overrideArgs(rankToOverrideArgs.Key, rankToOverrideArgs.Value);
                })
                .AddTo(_disposables);
                _actions.Select(action => action.GetOnOverrideAttributeIdsObservable()).Merge().Subscribe(rankToOverrideAttributeIds => 
                {
                    _overrideAttributeIds(rankToOverrideAttributeIds.Key, rankToOverrideAttributeIds.Value);
                })
                .AddTo(_disposables);
            })
            .Select(_ => this);
        }
        public IObservable<ActionHandler> LoadAssets(IActionExecutor executor, IActionFactory factory, IEnumerable<ActionAnimation> animations)
        {
            var animationMap = animations.ToDictionary(a => a.Name);
            return Observable.WhenAll(_actions.Select(a =>
            {
                if (animationMap.ContainsKey(a.AnimationName)) a.SetAnimation(animationMap[a.AnimationName]);
                return a.LoadAssets(executor, factory);
            }))
            .Select(_ => this);
        }

        public int GetBaseRange()
        {
            return ((AbstractRangeAction)_rankToNormalActionMap[0]).MaxRange;
        }
        public bool HasDeadTriggerAction()
        {
            return _actions.Any(a => a.Trigger == Trigger.Dead);
        }

        public bool CanPrepareNextActiveAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var activeAction = _getActiveAction();
            if (activeAction == null) return false;
            return activeAction.GetTargets(actionTargetFinder, executor).Length > 0;
        }

        public bool PrepareNextNormalAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var success = _prepareNextAction(_getCurrentNormalAction(), actionTargetFinder, executor);
            // 通常トリガーアクションは対象が存在しない場合はスキップして次のアクションにしてしまう。
            if (!success) _switchNormalAction();
            return success;
        }
        public bool PrepareNextActiveAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _prepareNextAction(_getActiveAction(), actionTargetFinder, executor);
        }
        public bool PrepareNextFirstAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _prepareNextAction(_getFirstAction(), actionTargetFinder, executor);
        }
        public bool PrepareNextDeadAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _prepareNextAction(_getDeadAction(), actionTargetFinder, executor);
        }
        public bool PrepareNextEnemyDeadAction(IActionTargetFinder actionTargetFinder, IActionExecutor exetutor)
        {
            return _prepareNextAction(_getEnemyDeadAction(), actionTargetFinder, exetutor);
        }
        public bool PrepareNextFirstWithAnimationAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _prepareNextAction(_getFirstWithAnimationAction(), actionTargetFinder, executor);
        }
        public bool PrepareNextReceiveEventAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor, ActionEvent actionEvent)
        {
            if (actionEvent.DamageType.HasValue && actionEvent.DamageType.Value == DamageType.Physical)
            {
                return _prepareNextAction(_getPhysicalDamageAction(), actionTargetFinder, executor);
            }
            return false;
        }
        public bool _prepareNextAction(IAction nextAction, IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            if (nextAction != null)
            {
                if (!_canExecuteAction(nextAction)) return false;
                var targets = nextAction.GetTargets(actionTargetFinder, executor);
                if (targets.Length > 0)
                {
                    _currentAction = nextAction;
                    _currentTargetCandidates = targets;
                    return true;
                }
            }
            return false;
        }

        public string GetCurrentActionAnimationName()
        {
            return _currentAction.AnimationName;
        }
        public DamageType? GetCurrentActionDamageType()
        {
            return _currentAction.DamageType;
        }

        public void PreProcess(IActionExecutor executor)
        {
            _finishSuccessfully = false;
            _tryToPlaySE(_currentAction.SoundID);
            _currentAction.PreProcess(executor, _currentTargetCandidates);
        }
        public void ExecuteCurrentAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            _executeAction(actionTargetFinder, executor, _currentAction, _currentTargetCandidates);
        }

        void _executeAction(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IAction action, IActionTarget[] targetCandidates)
        {
            if (!_canExecuteAction(action)) return;
            _rankToCurrentActionNum[action.Rank]++;
            _tryToPlaySE(action.ExecutionSoundID);
            action.Execute(actionTargetFinder, executor, targetCandidates);
        }

        public void PostProcess(IActionExecutor executor)
        {
            _currentAction.PostProcess(executor);
            _finishSuccessfully = true;
            if (_currentAction.Trigger == Trigger.Normal) _switchNormalAction();
        }
        public bool TryToCancel(IActionExecutor executor)
        {
            if (_finishSuccessfully) return false;
            _currentAction.Cancel(executor);
            return true;
        }
        public void Reset(IActionExecutor executor)
        {
            foreach (var action in _actions.ToList())
            {
                action.Reset(executor);
                _rankToCurrentActionNum[action.Rank] = 0;
            }
        }
        bool _canExecuteAction(IAction action)
        {
            if (action.Trigger == Trigger.PhysicalDamage && action.ExecutableProbability.HasValue)
            {
                if (!GlobalContainer.RandomGenerator.Lot(action.ExecutableProbability.Value))
                {
                    return false;
                }
            }
            return !action.ExecutableNum.HasValue || _rankToCurrentActionNum[action.Rank] < action.ExecutableNum.Value;
        }

        void _switchNormalAction()
        {
            // ノーマルトリガーアクションの切り替え
            if (_currentAction != null) _sequence.Switch();
        }

        public bool ExecuteFirstActionDirectly(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var canAct = PrepareNextFirstAction(actionTargetFinder, executor);
            if (canAct) ExecuteCurrentAction(actionTargetFinder, executor);
            return canAct;
        }

        public bool ExecuteEnemyDeadActionDirectly(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var action = _getEnemyDeadAction();
            if (action != null)
            {
                if (!_canExecuteAction(action)) return false;
                var targets = action.GetTargets(actionTargetFinder, executor);
                if (targets.Length == 0) return false;
                _executeAction(actionTargetFinder, executor, action, targets);
                return true;
            }
            return false;
        }

        public int GetActiveActionLevel()
        {
            return _getActiveAction().Level;
        }

        public bool IsFirstHidden()
        {
            return _actions.Any(a => a.GetType() == typeof(DelayAppearanceAttack));
        }

        public bool IsTargetable()
        {
            return _currentAction != null ? !_currentAction.ExecutorTargetable.HasValue : true;
        }

        public IObservable<SoundEvent> GetOnCommandSoundObservable()
        {
            return _onCommandSoundSubject.AsObservable();
        }

        public void PauseOn()
        {
            if (_currentAction != null) _currentAction.EnterTimeStop();
        }

        public void PauseOff()
        {
            foreach (var action in _actions)
            {
                action.ExitTimeStop();
            }
        }

        public IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable()
        {
            return _actions.Select(a => a.GetOnTryCameraShakeObservable()).Merge();
        }

        IAction _getCurrentNormalAction()
        {
            var rank = _sequence.GetRank();
            // もしも該当ランクのノーマルトリガーアクションがなければランク0のものを利用。
            if (!_rankToNormalActionMap.ContainsKey(rank)) rank = 0;
            return _rankToNormalActionMap[rank];
        }
        IAction _getActiveAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.Active).FirstOrDefault();
        }
        IAction _getFirstAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.First).FirstOrDefault();
        }
        IAction _getDeadAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.Dead).FirstOrDefault();
        }
        IAction _getPhysicalDamageAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.PhysicalDamage).FirstOrDefault();
        }
        IAction _getEnemyDeadAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.EnemyDead).FirstOrDefault();
        }
        IAction _getFirstWithAnimationAction()
        {
            return _actions.Where(a => a.Trigger == Trigger.FirstWithAnimation).FirstOrDefault();
        }

        void _overrideArgs(byte rank, Dictionary<byte, int> overrideValues)
        {
            var action = _actions.FirstOrDefault(a => a.Rank == rank);
            if (action != null)
            {
                foreach (var kv in overrideValues)
                {
                    action.OverrideArg(kv.Key, kv.Value);
                }
            }
        }
        void _overrideAttributeIds(byte rank, uint overrideAttributeID)
        {
            var action = _actions.FirstOrDefault(a => a.Rank == rank);
            if (action != null)
            {
                action.OverrideAttributeId(overrideAttributeID);
            }
        }

        void _tryToPlaySE(uint? soundId)
        {
            if (soundId.HasValue)
            {
                _onCommandSoundSubject.OnNext(new SoundEvent()
                {
                    SoundID = soundId.Value,
                    Command = SoundCommand.Play,
                });
            }
        }
    }
}
