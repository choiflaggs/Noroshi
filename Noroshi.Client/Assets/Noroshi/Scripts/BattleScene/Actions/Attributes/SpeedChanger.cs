using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class SpeedChanger : AbstractAttribute
    {
        StatusBoostFactor _factor;

        public SpeedChanger(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
            _factor = new StatusBoostFactor() {
                ActionFrequency = (_speed - 1),
            };
        }

        public override void OnEnter(IActionTarget target)
        {
            target.SetSpeed(_speed);
            target.AddStatusBoosterFactor(_factor);
        }

        public override void OnExit(IActionTarget target)
        {
            target.SetSpeed(1);
            target.RemoveStatusBoosterFactor(_factor);
        }
        
        public override bool IsNegative { get { return false; } }
        float _speed { get { return (float)_arg1 / 100; } }
    }
}