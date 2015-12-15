using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class WideAttack : AbstractWideAttack
    {
        public WideAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected override int _attackMinRange { get { return _arg4; } }
        protected override int _attackMaxRange { get { return _arg5; } }
        protected override bool _isAsynchronous { get { return _arg6 > 0; } }
        protected override float? _energyDamageRatio { get { return _arg7 > 0 ? (float?)(_arg7 / 100.0f) : null; } }
        protected override byte? _executeAttributeNum { get { return _arg8 > 0 ? (byte?)_arg8 : null; } }
        protected override byte? _animationSequenceNumber { get { return _arg9 > 0 ? (byte?)_arg9 : null; } }
        // TODO
        // 現状WideAttack系はActionViewしか使用していないので一旦固定値で、カメラ揺れなど必要になった場合対応予定
        protected override ActionEffectType? _actionEffectType { get { return ActionEffectType.ViewOnly; } }
    }
}