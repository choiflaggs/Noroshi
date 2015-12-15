namespace Noroshi.BattleScene.Actions.Attributes
{
    public class Charm : AbstractAttribute
    {
        public override bool IsNegative { get { return true; } }
        
        public Charm(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }
        
        public override void OnEnter(IActionTarget target)
        {
            if (_exitDamageRatio.HasValue)
            {
                _accumulationDamge = (int)(target.MaxHP * _exitDamageRatio.Value);
            }
            // この状態異常にかかったタイミングで自身を味方 -> 敵, 敵 -> 味方に反転させる
            target.SetCurrentForceReverse();
        }
        
        public override void OnExit(IActionTarget target)
        {
            _accumulationDamge = 0;
            // 反転したForceを元に戻す
            target.SetCurrentForceOriginal();
        }

        public override void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent)
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
        
        int _accumulationDamge;
        float? _exitDamageRatio { get { return _arg1 > 0 ? (float?)_arg1 / 100 : null; } }
    }
}
