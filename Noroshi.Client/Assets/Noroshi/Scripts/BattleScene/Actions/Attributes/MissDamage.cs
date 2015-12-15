namespace Noroshi.BattleScene.Actions.Attributes
{
    public class MissDamage : AbstractAttribute
    {
        public MissDamage(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnEnter(IActionTarget target)
        {
        }

        public override void OnExit(IActionTarget target)
        {
        }

        public override bool IsNegative { get {  return true; } }
    }
}