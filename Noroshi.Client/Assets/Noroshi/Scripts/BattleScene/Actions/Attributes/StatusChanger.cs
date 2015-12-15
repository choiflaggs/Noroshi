using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class StatusChanger : AbstractAttribute
    {
        enum BreakType
        {
            Armor = 1,
            MagicRegistance = 2,
        }

        StatusForceSetter.Factor _factor;

        public StatusChanger(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
            _factor = new StatusForceSetter.Factor()
            {
                Armor = _breakType == BreakType.Armor ? (int?)0 : null,
                MagicRegistance = _breakType == BreakType.MagicRegistance ? (int?)0 : null,
            };
        }

        public override void OnEnter(IActionTarget target)
        {
            target.AddStatusBreakerFactor(_factor);
        }

        public override void OnExit(IActionTarget target)
        {
            target.RemoveStatusBreakerFactor(_factor);
        }

        public override bool IsNegative { get { return true; } }
        BreakType _breakType { get { return (BreakType)_arg1; } }
    }
}
