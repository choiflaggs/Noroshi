using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    /// <summary>
    /// 他アクションの設定値を上書きするアクション。
    /// </summary>
    public class OverrideArg : AbstractAction
    {
        Subject<KeyValuePair<byte, Dictionary<byte, int>>> _onOverrideArgsSubject = new Subject<KeyValuePair<byte, Dictionary<byte, int>>>();
        Subject<KeyValuePair<byte, uint>> _onOverrideAttributeIdsSubject = new Subject<KeyValuePair<byte, uint>>();

        public OverrideArg(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        /// 上書き対象アクションランク。
        byte _actionRank { get { return (byte)_arg1; } }
        /// 上書き対象アクション番号（$"Arg{_argNo}" が上書かれる）
        byte? _argNo { get { return _arg2 > 0 ? (byte?)_arg2 : null; } }
        /// 上書き対象の状態異常ID(_attributeIdが上書かれる)
        uint? _overrideAttributeId { get { return _arg3 > 0 ? (uint?)_arg3 : null; } }

        public override IObservable<KeyValuePair<byte, Dictionary<byte, int>>> GetOnOverrideArgsObservable()
        {
            return _onOverrideArgsSubject.AsObservable();
        }
        public override IObservable<KeyValuePair<byte, uint>> GetOnOverrideAttributeIdsObservable()
        {
            return _onOverrideAttributeIdsSubject.AsObservable();
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return new[] { actionTargetFinder.GetExecutorAsTarget(executor) };
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            if (_argNo.HasValue)
            {
                // 上書き実行。
                _onOverrideArgsSubject.OnNext(new KeyValuePair<byte, Dictionary<byte, int>>(_actionRank, new Dictionary<byte, int>
                {
                    { _argNo.Value, _getOverrideValue() },
                }));
            }
            if (_overrideAttributeId.HasValue)
            {
                _onOverrideAttributeIdsSubject.OnNext(new KeyValuePair<byte, uint>(_actionRank, _overrideAttributeId.Value));
            }
        }

        /// 上書き値。
        protected int _getOverrideValue()
        {
            return (int)_getLevelDrivenParam1();
        }
    }
}
