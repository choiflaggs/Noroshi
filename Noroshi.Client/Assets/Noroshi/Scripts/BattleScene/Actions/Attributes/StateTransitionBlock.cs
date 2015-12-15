namespace Noroshi.BattleScene.Actions.Attributes
{
    public class StateTransitionBlock : AbstractAttribute
    {
        StateTransitionBlocker.Factor _factor;

        public StateTransitionBlock(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
            _factor = new StateTransitionBlocker.Factor()
            {
                ActiveActionBlock = _activeActionBlock,
                MagicActionBlock = _magicActionBlock,
                ProceedBlock = _proceedBlock,
            };
        }

        public override void OnEnter(IActionTarget target)
        {
            target.AddStateTransitionBlockerFactor(_factor);
        }

        public override void OnExit(IActionTarget target)
        {
            target.RemoveStateTransitionBlockerFactor(_factor);
        }
        
        public override bool IsNegative { get { return false; } }
        bool _activeActionBlock { get { return _arg1 > 0; } }
        bool _magicActionBlock { get { return _arg2 > 0; } }
        bool _proceedBlock { get { return _arg3 > 0; } }
    }
}
